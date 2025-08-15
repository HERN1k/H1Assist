using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using Application.Interfaces;
using Domain.ValueObjects;

namespace Application.Services
{
    internal partial class HtmlManagerService : IHtmlManagerService
    {
        private readonly ICacheService _cache;
        private readonly IImageEditorService _imageEditor;
        private readonly string _baseEKatalogUrl = "https://ek.ua/";
        private static readonly string[] _propertiesToSave = 
            [ "margin-top", "margin-bottom", "margin", "font-size", "font-weight", 
            "text-align", "display", "position", "height", "width" ];
        private const string _alloStyles = """
            .p-description {
                height: auto;
                position: relative;
            }
            .p-view__description {
                order: 0;
                flex: 100% 1 1;
                width: 100%;
                margin-right: 0;
                margin-left: 0;
                box-shadow: none;
                background: transparent;
            }
            .p-description__content {
                position: relative;
                max-height: 1026px;
                font-size: 16px;
                line-height: 24px;
                color: #929292;
                overflow: hidden;
                transition: max-height 1s;
            }
            .p-description__content__inner {
            }
            .l-container {
                max-width: 100% !important;
            }
            """;

        public HtmlManagerService(ICacheService cache, IImageEditorService imageEditor)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this._imageEditor = imageEditor ?? throw new ArgumentNullException(nameof(imageEditor));
        }

        public async Task<IDocument> GetDocumentAsync(string url)
        {
            if (_cache.TryGetValue(url, out IDocument? value))
            {
                return value!;
            }

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var document = await context.OpenAsync(url);

            _cache.SetValue(url, document);

            return document;
        }

        public async Task<IDocument> GetDocumentAsync(string productName, Language language, ExternalService service)
        {
            string url = service switch
            {
                ExternalService.EKatalog => ConfigureEKatalogUri(productName, language),
                _ => throw new NotSupportedException($"Service {service} is not supported.")
            };

            if (_cache.TryGetValue(url, out IDocument? value))
            {
                return value!;
            }

            var config = Configuration.Default.WithDefaultLoader();
            var context = BrowsingContext.New(config);

            var document = await context.OpenAsync(url);

            _cache.SetValue(url, document);

            return document;
        }

        public List<ProductCharacteristic> ParseEKatalogCharacteristicsAsync(IDocument? document)
        {
            List<ProductCharacteristic> result = new List<ProductCharacteristic>();

            if (document == null)
            {
                return result;
            }

            var table = document.GetElementById("help_table")
                ?.Children.ElementAtOrDefault(0)
                ?.Children.ElementAtOrDefault(1);

            if (table == null)
            {
                return result;
            }

            CleanEKatalogCharacteristicsAttributes(table);

            var rows = table.QuerySelectorAll("tr[valign=top]")
                .Where(tr => tr.Children.Length == 2);

            foreach (var row in rows)
            {
                var keyTd = row.Children[0];
                var valueTd = row.Children[1];

                string key = keyTd.TextContent?.Trim() ?? string.Empty;
                string value = valueTd.InnerHtml.Trim() ?? string.Empty;

                if (key.Contains("E-Katalog", StringComparison.InvariantCultureIgnoreCase))
                {
                    key = string.Empty;
                }

                if (value.Equals("<img alt=\"\">", StringComparison.InvariantCultureIgnoreCase) ||
                    value.Equals("<img>", StringComparison.InvariantCultureIgnoreCase))
                {
                    value = "✔";
                }

                if (value.EndsWith("<br>", StringComparison.InvariantCultureIgnoreCase))
                {
                    value = value.Substring(0, value.Length - 4);
                }

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    result.Add(new ProductCharacteristic(key, value));
                }
            }

            return result;
        }

        public async Task<string> DescriptionClean(string description)
        {
            return await CleanStylesAsync(description);
        }

        public async Task<(string, Dictionary<string, string>)> CleanDescriptionHtmlAsync(IElement descriptionElement, ExternalService service, string? newMediaFolderPath = "")
        {
            if (descriptionElement == null)
                return (string.Empty, new Dictionary<string, string>());

            Dictionary<string, string> externalMedia = new Dictionary<string, string>();
            bool changeURL = !string.IsNullOrEmpty(newMediaFolderPath);

            RemoveCommentsRecursive(descriptionElement);
            CleanEmptyElements(descriptionElement);
            RemoveUnwantedAttributes(descriptionElement);
            RemoveStylesComments(descriptionElement, externalMedia, changeURL, newMediaFolderPath);
            await CreateVideoPoster(descriptionElement, externalMedia, changeURL, newMediaFolderPath);
            ProcessMediaURLs(descriptionElement, externalMedia, changeURL, newMediaFolderPath);
            
            return (AddInjectStyleFunction(descriptionElement, service), externalMedia);
        }

        private static void RemoveStylesComments(IElement description, Dictionary<string, string> externalMedia, bool changeURL, string? newMediaFolderPath = "")
        {
            foreach (var styleEl in description.QuerySelectorAll("style"))
            {
                if (!string.IsNullOrWhiteSpace(styleEl.TextContent))
                {
                    string cleaned = StyleCommentRegex().Replace(styleEl.TextContent, string.Empty);

                    if (changeURL)
                    {
                        cleaned = StyleUrlRegex().Replace(cleaned, m =>
                        {
                            string oldUrl = m.Groups["url"].Value;
                            string newUrl = ReplaceAndTrackUrl(oldUrl, externalMedia, newMediaFolderPath);
                            return $"url('{newUrl}')";
                        });
                    }

                    styleEl.TextContent = cleaned.Trim();
                }
            }
        }

        private static void ProcessMediaURLs(IElement description, Dictionary<string, string> externalMedia, bool changeURL, string? newMediaFolderPath = "")
        {
            foreach (var img in description.QuerySelectorAll("img"))
            {
                if (changeURL)
                    ProcessMediaTag(img, "src", externalMedia, newMediaFolderPath);

                img.RemoveAttribute("alt");
                img.SetAttribute(
                    "style",
                    (img.GetAttribute("style") ?? string.Empty) + "width: inherit; height: inherit;"
                );
            }

            foreach (var video in description.QuerySelectorAll("video"))
            {
                if (changeURL)
                {
                    ProcessMediaTag(video, "src",  externalMedia, newMediaFolderPath);
                }

                video.RemoveAttribute("alt");
                video.SetAttribute(
                    "style",
                    (video.GetAttribute("style") ?? string.Empty) + " width: inherit; height: inherit;"
                );
            }

            if (changeURL)
            { 
                foreach (var el in description.QuerySelectorAll("*"))
                {
                    var style = el.GetAttribute("style");
                    if (!string.IsNullOrWhiteSpace(style))
                    {
                        string updated = StyleUrlRegex().Replace(style, m =>
                        {
                            string oldUrl = m.Groups["url"].Value;
                            string newUrl = ReplaceAndTrackUrl(oldUrl, externalMedia, newMediaFolderPath);
                            return $"url('{newUrl}')";
                        });

                        el.SetAttribute("style", updated);
                    }
                }
            }
        }

        private async Task CreateVideoPoster(IElement description, Dictionary<string, string> externalMedia, bool changeURL, string? newMediaFolderPath = "")
        {
            int index = 0;
            foreach (var video in description.QuerySelectorAll("video"))
            {
                if (changeURL)
                {
                    string? videoSrc = video.GetAttribute("src")?.Trim();

                    if (!string.IsNullOrEmpty(videoSrc))
                    {
                        string posterName = string.Concat("poster-", index.ToString(), ".jpg");

                        byte[] image = await _imageEditor.CreatePosterForVideoAsync(ImageExtension.JPG, videoSrc, posterName);

                        string imageBase64 = string.Concat("data:image/jpg;base64,", Convert.ToBase64String(image));

                        externalMedia.TryAdd(imageBase64, posterName);

                        video.RemoveAttribute("poster");
                        video.SetAttribute("poster", string.Concat(newMediaFolderPath, posterName));
                    }
                }

                index++;
            }
        }

        private static string AddInjectStyleFunction(IElement description, ExternalService service)
        {
            var styles = description
                .QuerySelectorAll("style")
                .ToList();

            string externalStyles = service switch
            {
                ExternalService.Allo => _alloStyles,
                _ => string.Empty
            };

            string styleString = string.Concat(externalStyles, string.Join(' ', styles.Select(style => style.InnerHtml)));

            foreach (var style in styles)
            {
                style.Remove();
            }

            return string.Concat(
                $$"""
                <script>
                    function injectStyle() {
                        const styleEl = document.createElement('style');
                        styleEl.type = 'text/css';
                        styleEl.textContent = `{{styleString
                            .Replace("\\", "\\\\")
                            .Replace("`", "\\`")
                            .Replace("${", "\\${")}}`;
    
                        const currentScript = document.currentScript;
                        currentScript.parentNode.insertBefore(styleEl, currentScript.nextSibling);
                    };

                    injectStyle();
                </script>
                """,
                description.InnerHtml
            );
        }

        private static void ProcessMediaTag(IElement element, string attrName, Dictionary<string, string> externalList, string? folderPath = "")
        {
            var url = element.GetAttribute(attrName)?.Trim();
            if (string.IsNullOrEmpty(url))
                return;

            string newUrl = ReplaceAndTrackUrl(url, externalList, folderPath);
            element.SetAttribute(attrName, newUrl);
        }

        private static string ReplaceAndTrackUrl(string oldUrl, Dictionary<string, string> externalList, string? folderPath = "")
        {
            string cleanUrl = oldUrl.Split('?')[0];
            string fileName = Path.GetFileNameWithoutExtension(cleanUrl);

            externalList.TryAdd(oldUrl, Path.GetFileName(cleanUrl));

            return string.Concat(folderPath?.TrimEnd('/') ?? string.Empty, "/", fileName, ImageExtension.JPG.Value);
        }

        private static void RemoveCommentsRecursive(INode node)
        {
            for (int i = node.ChildNodes.Length - 1; i >= 0; i--)
            {
                var child = node.ChildNodes[i];
                if (child.NodeType == NodeType.Comment)
                {
                    node.RemoveChild(child);
                }
                else
                {
                    RemoveCommentsRecursive(child);
                }
            }
        }

        private static async Task<string> CleanStylesAsync(string html)
        {
            var config = Configuration.Default.WithCss();
            var context = BrowsingContext.New(config);

            var document = await context.OpenAsync(req => req.Content(html));

            var styledElements = document.All.Where(el => el.HasAttribute("style"));

            var cssParser = new CssParser();

            foreach (var element in styledElements)
            {
                var styleContent = element.GetAttribute("style");

                if (string.IsNullOrWhiteSpace(styleContent))
                    continue;

                ICssStyleDeclaration styleDeclaration = cssParser.ParseDeclaration(styleContent);

                var allowedStyles = new List<string>();

                foreach (var propertyName in _propertiesToSave)
                {
                    var value = styleDeclaration.GetPropertyValue(propertyName);

                    if (!string.IsNullOrWhiteSpace(value))
                    {
                        allowedStyles.Add($"{propertyName}: {value}");
                    }
                }

                if (allowedStyles.Count == 0)
                {
                    element.RemoveAttribute("style");
                }
                else
                {
                    element.SetAttribute("style", string.Join("; ", allowedStyles));
                }
            }

            return document?.Body?.InnerHtml ?? string.Empty;
        }

        private static void CleanEKatalogCharacteristicsAttributes(IElement element)
        {
            foreach (var child in element.QuerySelectorAll("*"))
            {
                if (child.TagName == "TR")
                    continue;

                var attributes = child.Attributes.ToList();

                foreach (var attr in attributes)
                {
                    if (child.TagName == "A" && attr.Name == "href")
                        continue;

                    if (child.TagName == "A" && attr.Name == "link")
                    {
                        child.RemoveAttribute("href");
                        child.SetAttribute("href", child.GetAttribute("link"));
                        child.RemoveAttribute("link");

                        continue;
                    }

                    if (child.TagName == "A" && attr.Name == "target")
                        continue;

                    child.RemoveAttribute(attr.Name);
                }
            }
        }

        private static void CleanEmptyElements(IElement element)
        {
            for (int i = element.Children.Length - 1; i >= 0; i--)
            {
                var child = element.Children[i];

                CleanEmptyElements(child);

                bool hasText = !string.IsNullOrWhiteSpace(child.TextContent);
                bool hasChildren = child.Children.Length > 0;

                bool hasUsefulAttrs = child.HasAttribute("src");

                if (!hasText && !hasChildren && !hasUsefulAttrs)
                {
                    element.RemoveChild(child);
                }
            }
        }

        private static void RemoveUnwantedAttributes(IElement element)
        {
            foreach (var el in element.QuerySelectorAll("*"))
            {
                var attrsToRemove = new List<string>();

                foreach (var attr in el.Attributes)
                {
                    if (attr.Name.StartsWith("data-"))
                    {
                        attrsToRemove.Add(attr.Name);
                    }

                    if (attr.Name == "style" && string.IsNullOrWhiteSpace(attr.Value))
                    {
                        attrsToRemove.Add(attr.Name);
                    }
                }

                foreach (var attrName in attrsToRemove)
                {
                    el.RemoveAttribute(attrName);
                }
            }
        }

        private string ConfigureEKatalogUri(string productName, Language language)
        {
            StringBuilder sb = new StringBuilder(_baseEKatalogUrl);

            sb.Append(language == Language.RU ? "ru/" : "ua/");
            sb.Append(string.Concat(Regex.Replace(productName, @"[^a-zA-Z0-9\W_]", "")
                .Trim()
                .Replace(" ", "-")
                .ToUpperInvariant(), ".htm"));

            return sb.ToString();
        }

        [GeneratedRegex(@"/\*[\s\S]*?\*/")]
        private static partial Regex StyleCommentRegex();

        [GeneratedRegex(@"url\(['""]?(?<url>[^'"")]+)['""]?\)", RegexOptions.IgnoreCase, "ru-UA")]
        private static partial Regex StyleUrlRegex();
    }
}
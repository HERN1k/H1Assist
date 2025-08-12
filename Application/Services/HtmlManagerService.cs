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
    internal class HtmlManagerService : IHtmlManagerService
    {
        private readonly ICacheService _cache;
        private readonly string _baseEKatalogUrl = "https://ek.ua/";
        private readonly string _baseBrainUrl = "https://brain.com.ua/";
        private static readonly string[] _propertiesToSave = 
            [ "margin-top", "margin-bottom", "margin", "font-size", "font-weight", 
            "text-align", "display", "position", "height", "width" ];

        public HtmlManagerService(ICacheService cache)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
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

        public (string, List<string>) CleanDescriptionHtml(IElement descriptionElement, string newImgFolderPath)
        {
            if (descriptionElement == null)
                return (string.Empty, new List<string>());

            RemoveCommentsRecursive(descriptionElement);
            CleanEmptyElements(descriptionElement);
            RemoveUnwantedAttributes(descriptionElement);

            List<string> externalImages = new List<string>();

            foreach (var img in descriptionElement.QuerySelectorAll("img"))
            {
                img.RemoveAttribute("alt");

                var src = img.GetAttribute("src");
                var style = img.GetAttribute("style");
                if (!string.IsNullOrEmpty(src))
                {
                    externalImages.Add(src);

                    img.SetAttribute("style", string.Concat(style, "width: inherit; height: inherit;"));
                    img.SetAttribute("src", string.Concat(
                        newImgFolderPath.TrimEnd('/'),
                        "/",
                        Path.GetFileNameWithoutExtension(src),
                        ".jpg"
                    ));
                }
            }

            return (descriptionElement.InnerHtml, externalImages);
        }

        public async Task<string> DescriptionClean(string description)
        {
            return await CleanStylesAsync(description);
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
    }
}
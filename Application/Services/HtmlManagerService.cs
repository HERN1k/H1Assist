using System.Data;
using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Css.Dom;
using AngleSharp.Css.Parser;
using AngleSharp.Dom;
using AngleSharp.Io.Network;
using Application.DTOs;
using Application.Interfaces;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using WebMarkupMin.Core;

namespace Application.Services
{
    internal partial class HtmlManagerService : IHtmlManagerService
    {
        private readonly ICacheService _cache;
        private readonly IApiService _apiService;
        private readonly IImageEditorService _imageEditor;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<HtmlManagerService> _logger;
        private readonly ExternalService[] _onlyScrapeOpen = new ExternalService[1] { ExternalService.Comfy };
        private readonly string _baseEKatalogUrl = "https://ek.ua/";
        private static readonly string[] _propertiesToSave = 
            [ "margin-top", "margin-bottom", "margin", "font-weight", 
            "text-align", "display", "position", "height", "width" ];
        private const string _baseStylesForDescriptionClean = """
            <style>
                ul {
                    list-style: none;
                    padding-left: 0;
                }
                ul li {
                    position: relative;
                    padding-left: 3em;
                }
                ul li::before
                {
                    background: #019F01;
                    border-radius: 50%;
                    content: "";
                    display: inline-block;
                    height: 8px;
                    left: 1.5em;
                    position: absolute;
                    top: 6px;
                    width: 8px;
                }
                ol {
                    counter-reset: list-counter;
                    list-style: none;
                    padding-left: 0;
                }
                ol li {
                    counter-increment: list-counter;
                    position: relative;
                    padding-left: 2em;
                    margin-bottom: 0.5em;
                }
                ol li::before {
                    content: counter(list-counter);
                    position: absolute;
                    left: 0;
                    top: 0;
                    width: 1.5em;
                    height: 1.5em;
                    background-color: #019F01;
                    color: white;
                    border-radius: 50%;
                    display: flex;
                    align-items: center;
                    justify-content: center;
                    font-weight: bold;
                    font-size: 0.9em;
                }
            </style>
            """;
        private const string _baseStyles = """
            .main-product-description-wrapper {
                font-family: 'Averta CY', sans-serif, !important;
                width: 100%; 
                height: auto;
                margin: 0 auto; 
                display: block;
            }
            .main-product-description-wrapper * {
                font-family: 'Averta CY', sans-serif !important;
            }
            """;
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
        private const string _foxtrotStyles = """
            .product-about {
                display: flex !important;
            }
            .contenteditable-area h3 {
                font-size: 24px;
                display: block;
                width: 100%;
                margin-bottom: 25px;
                font-weight: 500;
            }
            .about-detail h3 {
                font-size: 24px;
                display: block;
                width: 100%;
                margin-bottom: 25px;
                font-weight: 500;
            }
            .contenteditable-area p {
                font-size: 16px;
                line-height: 1.4;
                margin-bottom: 25px;
                width: 100%;
            }
            .about-detail p {
                font-size: 16px;
                line-height: 1.4;
                margin-bottom: 25px;
                width: 100%;
            }
            .product-about p {
                font-size: 16px;
                line-height: 1.4;
                margin-bottom: 25px;
                width: 100%;
            }
            """;
        private const string _brainStyles = """
            .description-block {
                display: flex !important;
                flex-direction: column;
                min-height: 200px;
                position: relative;
                width: 100% !important;
            }
            .background-image {
                order: 2 !important;
                width: 100% !important;
            }
            .background-image img {
                display: block !important;
                margin: 0 auto !important;
                width: 100% !important;
            }
            .description-block .description-content {
                order: 1 !important;
                width: 100% !important;
            }
            .description-content .description-title {
                color: #000;
                font-size: 21px;
                font-weight: 700;
                line-height: 34px;
                padding: 15px 2% 4px;
                text-align: center;
                width: 100% !important;
            }
            .description-content .description-text {
                color: #000;
                font-size: 16px;
                font-weight: 400;
                line-height: 20px;
                margin-bottom: 14px;
                padding: 4px 5% 0;
                text-align: center;
                width: 100% !important;
            }
            """;
        private const string _comfyStyles = """
            .gen-sect__body {
                max-width: 100% !important;
                line-height: 2.5rem;
            }
            .gen-sect__title {
                margin: 0 auto !important;
                max-width: 100rem;
                text-align: center;
                font-weight: 600;
                font-size: 1.8rem;
                line-height: 2rem;
                margin-bottom: 1.6rem;
            }
            .bc-description__content {
                z-index: 1;
                font-size: 1.6rem;
                font-weight: 400;
                overflow: hidden;
            }
            @media (min-width: 1024px) {
                .gen-sect__header {
                    display: flex !important;
                    flex-wrap: wrap;
                    align-items: baseline;
                    gap: 1rem;
                    margin-bottom: 3.2rem;
                }
            }
            @media (min-width: 1024px) {
                .bc-description__content--expanded {
                    position: relative;
                    margin-top: 1.8rem;
                    max-height: none;
                }
                .bc-description__content {
                    position: relative;
                    margin-top: 1.8rem;
                    max-height: none;
                }
            }
            """;

        public HtmlManagerService(
            ICacheService cache, 
            IApiService apiService,
            IImageEditorService imageEditor,
            IHttpClientFactory httpClientFactory,
            ILogger<HtmlManagerService> logger
        )
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this._apiService = apiService ?? throw new ArgumentNullException(nameof(apiService));
            this._imageEditor = imageEditor ?? throw new ArgumentNullException(nameof(imageEditor));
            this._httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<IDocument?> GetDocumentAsync(string url, ExternalService service, bool useCache = true)
        {
            string cacheKey = $"Document_{service}_{url}";

            if (useCache && _cache.TryGetValue(cacheKey, out string? value))
            {
                return await GetBrowsingContext().OpenAsync(req => req.Content(value));
            }

            IDocument? document = null;
            if (_onlyScrapeOpen.Contains(service))
            {
                document = await GetDocumentUsingScrapeAsync(url);
            } 
            else
            {
                document = await GetBrowsingContext().OpenAsync(url);
            }

            if (useCache)
                if (document != null)
                    _cache.SetValue(cacheKey, document.ToHtml());

            return document;
        }

        public async Task<IDocument> GetDocumentAsync(string productName, Language language, ExternalService service, bool useCache = true)
        {
            string url = service switch
            {
                ExternalService.EKatalog => ConfigureEKatalogUri(productName, language),
                _ => throw new NotSupportedException($"Service {service} is not supported.")
            };

            if (useCache && _cache.TryGetValue(url, out IDocument? value))
            {
                return value!;
            }

            IDocument document = await GetBrowsingContext().OpenAsync(url);

            if (useCache)
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

        public async Task<CleanDescriptionHtmlDto> CleanDescriptionHtmlAsync(IElement descriptionElement, ExternalService service, string? newMediaFolderPath = "", bool useCache = true)
        {
            if (descriptionElement == null)
                return new CleanDescriptionHtmlDto(string.Empty, new Dictionary<string, string>());

            string cacheKey = $"CleanDescriptionHtml_{service}_{newMediaFolderPath}";

            if (useCache && _cache.TryGetValue(cacheKey, out CleanDescriptionHtmlDto? value))
                if (value != null)
                    return value;

            try
            {
                Dictionary<string, string> externalMedia = new Dictionary<string, string>();
                bool changeURL = !string.IsNullOrEmpty(newMediaFolderPath);

                SetSrcFromAttribute(descriptionElement);
                await LoadExternalCss(descriptionElement);
                RemoveCommentsRecursive(descriptionElement);
                CleanEmptyElements(descriptionElement);
                RemoveUnwantedAttributes(descriptionElement);
                RemoveUnusedStyleClasses(descriptionElement);
                RemoveStylesComments(descriptionElement, externalMedia, changeURL, newMediaFolderPath); 
                await CreateVideoPoster(descriptionElement, externalMedia, changeURL, newMediaFolderPath);
                ProcessMediaURLs(descriptionElement, externalMedia, changeURL, newMediaFolderPath);
                string resultHtml = MinifyHtml(AddInjectStyleFunction(descriptionElement, service));

                CleanDescriptionHtmlDto result = new CleanDescriptionHtmlDto(resultHtml, externalMedia);

                if (useCache)
                    _cache.SetValue(cacheKey, result);  

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cleaning description HTML.");
                return new CleanDescriptionHtmlDto(string.Empty, new Dictionary<string, string>());
            }
        }

        private async Task LoadExternalCss(IElement description)
        {
            List<IElement> links = description.QuerySelectorAll("link[rel='stylesheet']").ToList();

            foreach (var link in links)
            {
                string? href = link.GetAttribute("href");
                if (string.IsNullOrWhiteSpace(href))
                    continue;

                try
                {
                    string? cssContent = await _apiService.GetAsync(href);

                    Dictionary<string, Dictionary<string, string>> specialProperties = ExtractSpecialProperties(cssContent);

                    if (!string.IsNullOrWhiteSpace(cssContent))
                    {
                        ICssStyleSheet styleSheet = new CssParser().ParseStyleSheet(cssContent);

                        if (description.Owner != null && styleSheet.Rules.Length > 0)
                        {
                            IElement styleElement = description.Owner.CreateElement("style");

                            IEnumerable<ICssStyleRule> safeRules = styleSheet.Rules.OfType<ICssStyleRule>()
                                .Where(r => r.SelectorText != "*" && r.SelectorText != "html, body");
                            IEnumerable<ICssMediaRule> mediaRules = styleSheet.Rules.OfType<ICssMediaRule>();
                            IEnumerable<ICssRule> otherRules = styleSheet.Rules
                                .Where(r => r.Type != CssRuleType.Media && r.Type != CssRuleType.Style);

                            if (safeRules.Any())
                            {
                                AddRulesWithSpecialProperties(styleElement, safeRules, specialProperties);
                            }

                            foreach (var mediaRule in mediaRules)
                            {
                                IEnumerable<ICssStyleRule> mediaStyleRuless = mediaRule.Rules
                                    .OfType<ICssStyleRule>()
                                    .Where(r => r.SelectorText != "*" && r.SelectorText != "html, body");

                                styleElement.TextContent += $"@media {mediaRule.ConditionText} {{\n";

                                AddRulesWithSpecialProperties(styleElement, mediaStyleRuless, specialProperties);

                                styleElement.TextContent += "}\n";
                            }

                            if (otherRules.Any())
                            {
                                StringBuilder sb = new StringBuilder();

                                foreach (var r in otherRules)
                                {
                                    sb.AppendLine(r.CssText);
                                }

                                styleElement.TextContent += sb.ToString();
                            }

                            description.AppendChild(styleElement);
                        }

                        link.Remove();
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error loading external CSS from {href}", href);
                }
            }
        }

        private static Dictionary<string, Dictionary<string, string>> ExtractSpecialProperties(string? css)
        {
            Dictionary<string, Dictionary<string, string>> result = new Dictionary<string, Dictionary<string, string>>();

            if (string.IsNullOrWhiteSpace(css))
                return result;

            foreach (Match ruleMatch in RuleRegex().Matches(css))
            {
                string selector = ruleMatch.Groups["selector"].Value.Trim();
                string body = ruleMatch.Groups["body"].Value;

                var properties = new Dictionary<string, string>();

                foreach (Match propMatch in PropRegex().Matches(body))
                {
                    string name = propMatch.Groups["name"].Value.Trim();
                    string value = propMatch.Groups["value"].Value.Trim();

                    if (name.StartsWith("-webkit-") ||
                        name.StartsWith("-moz-") ||
                        name.StartsWith("-ms-") ||
                        name.StartsWith("-o-") ||
                        name.StartsWith("gap") ||
                        value.Contains("clamp(") ||
                        value.Contains("calc(") ||
                        value.Contains("var(") ||
                        value.Contains("linear-gradient("))
                    {
                        properties[name] = value;
                    }
                }

                if (properties.Count > 0)
                {
                    result[selector] = properties;
                }
            }

            return result;
        }

        private async Task<IDocument?> GetDocumentUsingScrapeAsync(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            try
            {
                StringBuilder sb = new StringBuilder("http://api.scrape.do/?url=");

                sb.Append(Uri.EscapeDataString(url));
                sb.Append("&token=");
                sb.Append(Environment.GetEnvironmentVariable("SCRAPE_DO_API_KEY") 
                    ?? throw new InvalidOperationException("SCRAPE_DO_API_KEY environment variable is not set."));

                string? content = await _apiService.GetAsync(sb.ToString());

                if (string.IsNullOrEmpty(content))
                    return null;

                return await GetBrowsingContext().OpenAsync(req => req.Content(content));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error connecting to Scrape.");
                throw;
            }
        }

        private IBrowsingContext GetBrowsingContext()
        {
            IConfiguration config = Configuration.Default
                .With(new HttpClientRequester(_httpClientFactory.CreateClient(nameof(IHtmlManagerService))))
                .WithJs()
                .WithDefaultLoader()
                .WithDefaultCookies();

            return BrowsingContext.New(config);
        }

        private static void SetSrcFromAttribute(IElement description)
        {
            foreach (var img in description.QuerySelectorAll("img"))
            {
                string? dataSrc = img.GetAttribute("data-src");
                if (!string.IsNullOrEmpty(dataSrc))
                {
                    img.RemoveAttribute("src");
                    img.SetAttribute("src", dataSrc);
                }
            }
        }

        private static void RemoveUnusedStyleClasses(IElement description)
        {
            HashSet<string> usedClasses = new HashSet<string>(
                description.QuerySelectorAll("*")
                    .SelectMany(e => e.ClassList)
                    .Where(c => !string.IsNullOrWhiteSpace(c))
            );

            if (usedClasses.Count == 0)
                return;

            CssParser cssParser = new CssParser();
            
            foreach (var styleEl in description.QuerySelectorAll("style").ToList())
            {
                Dictionary<string, Dictionary<string, string>> specialProperties = ExtractSpecialProperties(styleEl.InnerHtml);
                ICssStyleSheet sheet = cssParser.ParseStyleSheet(styleEl.InnerHtml);

                styleEl.TextContent = string.Empty;

                List<ICssRule> newRules = new List<ICssRule>();

                foreach (var rule in sheet.Rules)
                {
                    if (rule.Type == CssRuleType.Style)
                    {
                        var styleRule = (ICssStyleRule)rule;

                        var selectors = styleRule.SelectorText
                            .Split(',')
                            .Select(s => s.Trim())
                            .ToList();

                        var keptSelectors = selectors.Where(sel =>
                        {
                            var classMatches = ExtractCssClassNames().Matches(sel);
                            if (classMatches.Count == 0)
                                return true;
                            return classMatches.Cast<Match>()
                                                .Any(m => usedClasses.Contains(m.Groups[1].Value));
                        }).ToList();

                        if (keptSelectors.Count > 0)
                        {
                            styleRule.SelectorText = string.Join(", ", keptSelectors);
                            newRules.Add(styleRule);
                        }
                    }
                    else if (rule.Type == CssRuleType.Media)
                    {
                        var mediaRule = (ICssGroupingRule)rule;
                        var innerRules = new List<ICssRule>();

                        foreach (var inner in mediaRule.Rules)
                        {
                            if (inner.Type == CssRuleType.Style)
                            {
                                var styleRule = (ICssStyleRule)inner;

                                var selectors = styleRule.SelectorText?
                                    .Split(',')
                                    .Select(s => s.Trim())
                                    .ToList();

                                var keptSelectors = selectors?.Where(sel =>
                                {
                                    var classMatches = ExtractCssClassNames().Matches(sel);
                                    if (classMatches.Count == 0)
                                        return true;
                                    return classMatches.Cast<Match>()
                                                        .Any(m => usedClasses.Contains(m.Groups[1].Value));
                                }).ToList();

                                if (keptSelectors?.Count > 0)
                                {
                                    styleRule.SelectorText = string.Join(", ", keptSelectors);
                                    innerRules.Add(styleRule);
                                }
                            }
                            else
                            {
                                innerRules.Add(inner);
                            }
                        }

                        if (innerRules.Count > 0)
                        {
                            var newMedia = cssParser.ParseStyleSheet($"@media {((ICssMediaRule)mediaRule).ConditionText}{{{string.Join("", innerRules.Select(r => r.CssText))}}}");
                            foreach (var r in newMedia.Rules)
                                newRules.Add(r);
                        }
                    }
                    else
                    {
                        newRules.Add(rule);
                    }
                }

                IEnumerable<ICssStyleRule> styleRules = newRules.OfType<ICssStyleRule>();
                IEnumerable<ICssMediaRule> mediaRules = newRules.OfType<ICssMediaRule>();
                IEnumerable<ICssRule> otherRules = newRules.Where(r => r.Type != CssRuleType.Style && r.Type != CssRuleType.Media);

                AddRulesWithSpecialProperties(styleEl, styleRules, specialProperties);

                foreach (var rule in mediaRules)
                {
                    IEnumerable<ICssStyleRule> mediaStyleRules = rule.Rules.OfType<ICssStyleRule>();

                    styleEl.TextContent += $"@media {rule.ConditionText} {{\n";

                    AddRulesWithSpecialProperties(
                        element: styleEl,
                        rules: mediaStyleRules,
                        specialProperties: specialProperties); 

                    styleEl.TextContent += "}\n";
                }

                StringBuilder sb = new StringBuilder();
                foreach (var r in otherRules)
                {
                    sb.AppendLine(r.CssText);
                }

                styleEl.TextContent += sb.ToString();
            }
        }

        private static void AddRulesWithSpecialProperties(IElement element, IEnumerable<ICssStyleRule> rules, Dictionary<string, Dictionary<string, string>> specialProperties)
        {
            foreach (var rule in rules)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(rule.SelectorText);
                sb.Append(" { ");

                foreach (var prop in rule.Style)
                {
                    sb.Append($"{prop.Name}: {prop.Value}; ");
                }

                if (specialProperties.TryGetValue(rule.SelectorText, out var specialProps))
                {
                    foreach (var specialProp in specialProps)
                    {
                        sb.Append($"{specialProp.Key}: {specialProp.Value}; ");
                    }
                }

                sb.Append(" }\n");
                element.TextContent += sb.ToString();
            }
        }

        private string MinifyHtml(string html)
        {
            HtmlMinifier minifier = new HtmlMinifier(new HtmlMinificationSettings()
            {
                MinifyEmbeddedCssCode = true,
                MinifyEmbeddedJsCode = true
            });

            MarkupMinificationResult result = minifier.Minify(html, generateStatistics: false);

            if (result.Errors.Count > 0)
            {
                foreach (var error in result.Errors)
                {
                    _logger.LogError("Error during minification: {error}", error.Message);
                }
            }

            return result.MinifiedContent;
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
                    (img.GetAttribute("style") ?? string.Empty)
                );
            }

            foreach (var video in description.QuerySelectorAll("video"))
            {
                if (changeURL)
                {
                    ProcessMediaTag(video, "src",  externalMedia, newMediaFolderPath);
                }

                video.RemoveAttribute("alt");
                video.SetAttribute("muted", string.Empty);
                video.SetAttribute(
                    "style",
                    (video.GetAttribute("style") ?? string.Empty)
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
            if (!changeURL) return;

            int index = 0;
            foreach (var video in description.QuerySelectorAll("video"))
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

                index++;
            }
        }

        private string AddInjectStyleFunction(IElement description, ExternalService service)
        {
            List<IElement> styles = description.QuerySelectorAll("style").ToList();

            string externalStyles = service switch
            {
                ExternalService.Brain => _brainStyles,
                ExternalService.Foxtrot => _foxtrotStyles,
                ExternalService.Allo => _alloStyles,
                ExternalService.Comfy => _comfyStyles,
                _ => string.Empty
            };

            string styleString = string.Concat(_baseStyles, externalStyles, string.Join(' ', styles
                .Select(style => style.InnerHtml)));

            foreach (var style in styles)
                style.Remove();

            StringBuilder sb = new StringBuilder();
            
            sb.AppendLine("<style>");
            sb.AppendLine(styleString);
            sb.AppendLine("</style>");
            sb.AppendLine("<div class=\"main-product-description-wrapper\">");
            sb.AppendLine(description.InnerHtml);
            sb.AppendLine("</div>");

            string minifiedHtml = MinifyHtml(sb.ToString());

            return $$"""
                <isolated-product-description 
                    style="max-width: 100% !important; width: 100% !important; 
                           height: auto !important; margin: 0 auto !important; 
                           display: block !important; overflow: hidden;"
                ></isolated-product-description>

                <div style="display: none !important;">
                    <script>
                        class ProductDescription extends HTMLElement {
                            constructor() {
                                super();
                                this.attachShadow({ mode: 'open' });
                            }

                            connectedCallback() {
                                this.shadowRoot.innerHTML = `{{minifiedHtml}}`;
                            }
                        }

                        customElements.define('isolated-product-description', ProductDescription);
                    </script>
                </div>
                """;
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
            string fileName = Path.GetFileName(cleanUrl).Replace('_', '-').ToLowerInvariant();
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(cleanUrl).Replace('_', '-').ToLowerInvariant();

            externalList.TryAdd(oldUrl, fileName);

            if (oldUrl.EndsWith(".mp4", StringComparison.InvariantCultureIgnoreCase))
                return string.Concat(folderPath?.TrimEnd('/') ?? string.Empty, "/", fileName);

            if (fileName.EndsWith(ImageExtension.PNGString, StringComparison.InvariantCultureIgnoreCase))
                return string.Concat(folderPath?.TrimEnd('/') ?? string.Empty, "/", fileNameWithoutExtension, ImageExtension.PNG.Value);
            else
                return string.Concat(folderPath?.TrimEnd('/') ?? string.Empty, "/", fileNameWithoutExtension, ImageExtension.JPG.Value);
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

        private async Task<string> CleanStylesAsync(string html)
        {
            IConfiguration config = Configuration.Default.WithCss();
            IBrowsingContext context = BrowsingContext.New(config);

            IDocument document = await context.OpenAsync(req => req.Content(html));

            IEnumerable<IElement> styledElements = document.All.Where(el => el.HasAttribute("style"));

            CssParser cssParser = new CssParser();

            foreach (var element in styledElements)
            {
                string? styleContent = element.GetAttribute("style");

                if (string.IsNullOrWhiteSpace(styleContent))
                    continue;

                ICssStyleDeclaration styleDeclaration = cssParser.ParseDeclaration(styleContent);

                List<string> allowedStyles = new List<string>();

                foreach (var propertyName in _propertiesToSave)
                {
                    string? value = styleDeclaration.GetPropertyValue(propertyName);

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

            return MinifyHtml(string.Concat(_baseStylesForDescriptionClean, document?.Body?.InnerHtml ?? string.Empty));
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
                List<string> attrsToRemove = new List<string>();

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

        [GeneratedRegex(@"\.([a-zA-Z0-9_-]+)")]
        private static partial Regex ExtractCssClassNames();

        [GeneratedRegex(@"(?<selector>[^{]+)\{(?<body>[^}]+)\}")]
        private static partial Regex RuleRegex();

        [GeneratedRegex(@"(?<name>[-\w]+)\s*:\s*(?<value>([^;{}()]|\([^)]*\))+);?")]
        private static partial Regex PropRegex();
    }
}
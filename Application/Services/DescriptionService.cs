using System.Text;
using System.Text.RegularExpressions;
using AngleSharp;
using AngleSharp.Dom;
using Application.Interfaces;
using Domain.ValueObjects;

namespace Application.Services
{
    internal sealed class DescriptionService : IDescriptionService
    {
        private readonly ICacheService _cache;

        public DescriptionService(ICacheService cache)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<ProductDescription> GenerateAsync(string productName, Language language)
        {
            try
            {
                string cacheKey = $"{language}_{productName}";

                if (_cache.TryGetValue(cacheKey, out ProductDescription? value))
                {
                    return value ?? new ProductDescription();
                }

                StringBuilder sb = new StringBuilder("https://ek.ua/");

                sb.Append(language == Language.RU ? "ru/" : "ua/");
                sb.Append(string.Concat(Regex.Replace(productName, @"[^a-zA-Z0-9\W_]", "")
                    .Trim()
                    .Replace(" ", "-")
                    .ToUpperInvariant(), ".htm"));

                var config = Configuration.Default.WithDefaultLoader();
                var context = BrowsingContext.New(config);

                var document = await context.OpenAsync(sb.ToString());

                ProductDescription result = new ProductDescription()
                {
                    Characteristics = ParseCharacteristicsAsync(document),
                };

                _cache.SetValue(cacheKey, result);

                return result;
            }
            catch (Exception)
            {
                return new ProductDescription();
            }
        }
        
        public static List<ProductCharacteristic> ParseCharacteristicsAsync(IDocument? document)
        {
            var result = new List<ProductCharacteristic>();

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

            CleanAttributes(table);

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

        public static void CleanAttributes(IElement element)
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
    }
}
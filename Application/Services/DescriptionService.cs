using System.Collections.Frozen;

using AngleSharp.Dom;

using Application.DTOs;
using Application.Interfaces;

using Domain.ValueObjects;

namespace Application.Services
{
    internal sealed class DescriptionService : IDescriptionService
    {
        private readonly ICacheService _cache;
        private readonly IHtmlManagerService _htmlManager;
        
        public DescriptionService(ICacheService cache, IHtmlManagerService htmlManager)
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this._htmlManager = htmlManager ?? throw new ArgumentNullException(nameof(htmlManager));
        }

        public async Task<List<ProductCharacteristic>> GenerateCharacteristicsAsync(string productName, Language language)
        {
            try
            {
                IDocument document = await _htmlManager.GetDocumentAsync(productName, language, ExternalService.EKatalog);

                string cacheKey = $"Characteristics_{ExternalService.EKatalog}_{productName}_{language}";

                if (_cache.TryGetValue(cacheKey, out List<ProductCharacteristic>? value))
                {
                    return value ?? new List<ProductCharacteristic>();
                }

                value = _htmlManager.ParseEKatalogCharacteristicsAsync(document);

                _cache.SetValue(cacheKey, value);

                return value;
            }
            catch (Exception)
            {
                return new List<ProductCharacteristic>();
            }
        }

        public async Task<CleanDescriptionHtmlDto?> CleanHtmlAsync(string descriptionUrl, string? dirUrl = "")
        {
            if (string.IsNullOrWhiteSpace(descriptionUrl))
            {
                return null;
            }

            IDocument document = await _htmlManager.GetDocumentAsync(descriptionUrl, false);

            IElement? element = document.QuerySelectorAll(".p-description__content").ElementAtOrDefault(0);

            if (element == null)
            {
                return null;
            }

            return await _htmlManager.CleanDescriptionHtmlAsync(element, ExternalService.Allo, dirUrl);
        }
    }
}
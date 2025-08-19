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
        private readonly IImageEditorService _imageEditor;
        private readonly IApiService _api;

        public DescriptionService(
            ICacheService cache, 
            IHtmlManagerService htmlManager, 
            IImageEditorService imageEditor,
            IApiService api
        )
        {
            this._cache = cache ?? throw new ArgumentNullException(nameof(cache));
            this._htmlManager = htmlManager ?? throw new ArgumentNullException(nameof(htmlManager));
            this._imageEditor = imageEditor ?? throw new ArgumentNullException(nameof(imageEditor));
            this._api = api ?? throw new ArgumentNullException(nameof(api));
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

        public async Task<CleanDescriptionHtmlDto?> CleanHtmlAsync(string descriptionUrl, ExternalService service, string? dirUrl = "", bool useCache = true)
        {
            if (string.IsNullOrWhiteSpace(descriptionUrl))
            {
                return null;
            }

            IElement? element = await GetDescriptionHtmlAsync(descriptionUrl, service, useCache);

            if (element == null)
            {
                return null;
            }

            return await _htmlManager.CleanDescriptionHtmlAsync(element, service, dirUrl, useCache);
        }

        public async Task<byte[]> DownloadMediaAsync(string[] media, Dictionary<string, string> base64Media)
        {
            HashSet<string> validLinks = new HashSet<string>();
            HashSet<string> invalidLinks = new HashSet<string>();

            string sharedDir = "/app/shared";
            string id = Guid.NewGuid().ToString();
            string outputDir = Path.Combine(sharedDir, $"{id}_output");

            try
            {
                foreach (string link in media)
                {
                    if (string.IsNullOrWhiteSpace(link))
                        continue;

                    string fileName = Path.GetFileName(link);

                    if (string.IsNullOrWhiteSpace(fileName))
                        continue;

                    switch (Path.GetExtension(fileName).ToLowerInvariant())
                    {
                        case ImageExtension.JPEGString:
                        case ImageExtension.JPGString:
                        case ImageExtension.PNGString:
                        case ImageExtension.WEBPString:
                            validLinks.Add(link);
                            break;
                        case ".mp4":
                        case ".webm":
                        case ".avi":
                        case ".mov":
                        case ".mkv":
                        case ".flv":
                        case ".gif":
                            break; // Video formats skip
                        default:
                            validLinks.Add(link);
                            //invalidLinks.Add(link);
                            break;
                    }
                }

                //await _imageEditor.DownloadAndConvertImageFormatAsync(ImageExtension.JPG, validLinks.ToList(), outputDir);
                //await _api.DownloadImagesToDirectoryAsync(invalidLinks.ToList(), outputDir);
                await _api.DownloadImagesToDirectoryAsync(validLinks.ToList(), outputDir);

                foreach (var base64 in base64Media)
                {
                    if (string.IsNullOrWhiteSpace(base64.Key) || string.IsNullOrWhiteSpace(base64.Value))
                        continue;

                    await _imageEditor.SaveBase64Image(base64.Key, Path.Combine(outputDir, base64.Value));
                }

                return await _imageEditor.CreateZipInMemoryAsync(outputDir);
            }
            catch (Exception)
            {
                throw;
            }
            finally 
            {
                if (Directory.Exists(outputDir))
                    Directory.Delete(outputDir, recursive: true);
            }
        }

        private async Task<IElement?> GetDescriptionHtmlAsync(string url, ExternalService service, bool useCache = true)
        {
            if (string.IsNullOrWhiteSpace(url))
                return null;

            IDocument? document = await _htmlManager.GetDocumentAsync(url, service, useCache);

            if (document == null)
                return null;

            IElement? element = null;
            if (service.Equals(ExternalService.Comfy))
            {
                element = document.GetElementById("description");
            }
            else if (service.Equals(ExternalService.Brain))
            {
                element = document.QuerySelectorAll(".product-additional-description").FirstOrDefault();
            }
            else if (service.Equals(ExternalService.Foxtrot))
            {
                element = document.QuerySelectorAll(".product-about").FirstOrDefault();
                element ??= document.QuerySelectorAll(".product-about__container-for-content").FirstOrDefault();
            }
            else if (service.Equals(ExternalService.Allo))
            {
                element = document.QuerySelectorAll(".p-description__content").FirstOrDefault();
                element ??= document.GetElementById("extended-description");
            }

            return element ?? null;
        }
    }
}
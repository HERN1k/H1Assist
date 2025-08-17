using Application.DTOs;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IDescriptionService
    {
        Task<List<ProductCharacteristic>> GenerateCharacteristicsAsync(string productName, Language language);
        Task<CleanDescriptionHtmlDto?> CleanHtmlAsync(string descriptionUrl, ExternalService service, string? dirUrl = "", bool useCache = true);
        Task<byte[]> DownloadMediaAsync(string[] media, Dictionary<string, string> base64Media);
    }
}
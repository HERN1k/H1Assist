using AngleSharp.Dom;
using Application.DTOs;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IHtmlManagerService
    {
        Task<IDocument?> GetDocumentAsync(string url, ExternalService service, bool useCache = true);
        Task<IDocument> GetDocumentAsync(string productName, Language language, ExternalService service, bool useCache = true);
        List<ProductCharacteristic> ParseEKatalogCharacteristicsAsync(IDocument? document);
        Task<string> DescriptionClean(string description);
        Task<CleanDescriptionHtmlDto> CleanDescriptionHtmlAsync(IElement descriptionElement, ExternalService service, string? newMediaFolderPath = "", bool useCache = true);
    }
}
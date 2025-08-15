using AngleSharp.Dom;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IHtmlManagerService
    {
        Task<IDocument> GetDocumentAsync(string url);
        Task<IDocument> GetDocumentAsync(string productName, Language language, ExternalService service);
        List<ProductCharacteristic> ParseEKatalogCharacteristicsAsync(IDocument? document);
        Task<string> DescriptionClean(string description);
        Task<(string, Dictionary<string, string>)> CleanDescriptionHtmlAsync(IElement descriptionElement, ExternalService service, string? newMediaFolderPath = "");
    }
}
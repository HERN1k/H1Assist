using AngleSharp.Dom;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IHtmlManagerService
    {
        Task<IDocument> GetDocumentAsync(string url);
        Task<IDocument> GetDocumentAsync(string productName, Language language, ExternalService service);
        List<ProductCharacteristic> ParseEKatalogCharacteristicsAsync(IDocument? document);
        (string, List<string>) CleanDescriptionHtml(IElement descriptionElement, string newImgFolderPath);
        Task<string> DescriptionClean(string description);
    }
}
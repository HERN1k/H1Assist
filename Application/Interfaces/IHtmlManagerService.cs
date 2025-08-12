using System.Collections.Frozen;
using AngleSharp.Dom;
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IHtmlManagerService
    {
        Task<IDocument> GetDocumentAsync(string productName, Language language, ExternalService service);
        FrozenSet<ProductCharacteristic> ParseEKatalogCharacteristicsAsync(IDocument? document);
        Task<string> DescriptionClean(string description);
    }
}
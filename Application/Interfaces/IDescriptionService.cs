using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IDescriptionService
    {
        Task<ProductDescription> GenerateAsync(string productName, Language language);
    }
}
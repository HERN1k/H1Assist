using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface IDescriptionService
    {
        Task<List<ProductCharacteristic>> GenerateCharacteristicsAsync(string productName, Language language);
    }
}
using Domain.ValueObjects;

namespace Application.Interfaces
{
    public interface ISeoGeneratorService
    {
        string GenerateHeading(string productName, Language lang = Language.UA);
        string GenerateTitle(string productName, Language lang = Language.UA);
        string GenerateKeywords(string productName, Language lang = Language.UA);
        string GenerateDescription(string productName, Language lang = Language.UA);
    }
}
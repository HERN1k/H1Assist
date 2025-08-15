namespace Application.DTOs
{
    public sealed class CleanDescriptionHtmlDto(string value, Dictionary<string, string> externalImages)
    {
        public string Value { get; set; } = value ?? string.Empty;
        public Dictionary<string, string> ExternalImages { get; set; } = externalImages ?? new Dictionary<string, string>();
    }
}
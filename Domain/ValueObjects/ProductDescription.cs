namespace Domain.ValueObjects
{
    public struct ProductDescription(string description = "", bool isHtmlDescription = false)
    {
        public bool IsHtmlDescription { get; set; } = isHtmlDescription;
        public string Description { get; set; } = description;
    }
}
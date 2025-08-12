using System.Text.Json.Serialization;

namespace Domain.ValueObjects
{
    public struct ProductDescription
    {
        public bool IsHtmlDescription { get; set; }
        public string Description { get; set; }

        [JsonConstructor]
        public ProductDescription(string description = "", bool isHtmlDescription = false)
        {
            this.IsHtmlDescription = isHtmlDescription;
            this.Description = description;
        }
    }
}
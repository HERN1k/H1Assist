using Domain.ValueObjects;

namespace H1Assist.Models
{
    public sealed class ContentGeneratorViewModel
    {
        public bool ContentVisible { get; set; }
        public string ProductNameUA { get; set; } = string.Empty;
        public string HeadingUA { get; set; } = string.Empty;
        public string TitleUA { get; set; } = string.Empty;
        public string KeywordsUA { get; set; } = string.Empty;
        public string DescriptionUA { get; set; } = string.Empty;
        public string ProductNameRU { get; set; } = string.Empty;
        public string HeadingRU { get; set; } = string.Empty;
        public string TitleRU { get; set; } = string.Empty;
        public string KeywordsRU { get; set; } = string.Empty;
        public string DescriptionRU { get; set; } = string.Empty;
        public List<ProductCharacteristic> Characteristics { get; set; } = new List<ProductCharacteristic>();
    }
}
namespace Domain.ValueObjects
{
    public sealed class ProductDescription
    {
        public List<ProductCharacteristic> Characteristics { get; set; } = new List<ProductCharacteristic>();
        public bool IsHtmlDescription { get; set; } = false;
        public string Description { get; set; } = string.Empty;
    }
}
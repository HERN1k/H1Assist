namespace Domain.ValueObjects
{
    public readonly struct ProductCharacteristic
    {
        public string Name { get; }
        public string Value { get; }

        public ProductCharacteristic(string name, string value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return $"{Name}: {Value}";
        }
    }
}
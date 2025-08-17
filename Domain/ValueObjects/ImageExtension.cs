namespace Domain.ValueObjects
{
    public sealed class ImageExtension : IEquatable<ImageExtension>
    {
        public static readonly HashSet<string> ValidExtensions = new() {
            ".jpg", ".jpeg", ".png", ".webp", ".avif"
        };
        public static readonly ImageExtension JPG = new ImageExtension(".jpg");
        public static readonly ImageExtension JPEG = new ImageExtension(".jpeg");
        public static readonly ImageExtension PNG = new ImageExtension(".png");
        public static readonly ImageExtension WEBP = new ImageExtension(".webp");
        public static readonly ImageExtension AVIF = new ImageExtension(".avif");
        
        public const string JPGString = ".jpg";
        public const string JPEGString = ".jpeg";
        public const string PNGString = ".png";
        public const string WEBPString = ".webp";
        public const string AVIFString = ".avif";

        public string Value { get; } = string.Empty;
        public bool HasValue => !string.IsNullOrEmpty(Value);

        public ImageExtension(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Invalid image extension.", nameof(value));

            if (!ValidExtensions.Contains(value))
                throw new ArgumentException($"Invalid image extension: {value}. Valid extensions are: {string.Join(", ", ValidExtensions)}", nameof(value));

            Value = value;
        }

        public static bool TryParse(string? value, out ImageExtension? imageExtension)
        {
            if (string.IsNullOrWhiteSpace(value) || !value.Contains('.'))
            {
                imageExtension = null;
                return false;
            }

            string extension = Path.GetExtension(value)?.ToLowerInvariant() ?? "";

            if (!ValidExtensions.Contains(extension))
            {
                imageExtension = null;
                return false;
            }

            imageExtension = new ImageExtension(extension);

            return true;
        }

        public override bool Equals(object? obj)
        {
            if (obj is null) 
                return false;

            if (ReferenceEquals(this, obj)) 
                return true;

            if (obj.GetType() != GetType()) 
                return false;

            return Equals((ImageExtension)obj);
        }

        public bool Equals(ImageExtension? other) => other is not null && Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public override string ToString() => Value;
    }
}
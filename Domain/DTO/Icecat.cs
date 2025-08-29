using System.Text.Json;
using System.Text.Json.Serialization;
using System.Globalization;

namespace Domain.DTO
{
    public static class Icecat
    {
        public static IcecatJson? Deserialize(string json) => JsonSerializer.Deserialize<IcecatJson>(json, Converter.Settings);
    }

    public partial class IcecatJson
    {
        [JsonPropertyName("msg")]
        public string? Msg { get; set; }

        [JsonPropertyName("data")]
        public Data? Data { get; set; }
    }

    public partial class Data
    {
        [JsonPropertyName("GeneralInfo")]
        public GeneralInfo? GeneralInfo { get; set; }

        [JsonPropertyName("Image")]
        public Image? Image { get; set; }

        [JsonPropertyName("Multimedia")]
        public List<Multimedia> Multimedia { get; set; } = new List<Multimedia>();

        [JsonPropertyName("Gallery")]
        public List<Gallery> Gallery { get; set; } = new List<Gallery>();

        [JsonPropertyName("FeaturesGroups")]
        public List<FeaturesGroup> FeaturesGroups { get; set; } = new List<FeaturesGroup>();

        [JsonPropertyName("FeatureLogos")]
        public List<object> FeatureLogos { get; set; } = new List<object>();

        [JsonPropertyName("ReasonsToBuy")]
        public List<object> ReasonsToBuy { get; set; } = new List<object>();

        [JsonPropertyName("Reviews")]
        public List<object> Reviews { get; set; } = new List<object>();

        [JsonPropertyName("ProductRelated")]
        public List<ProductRelated> ProductRelated { get; set; } = new List<ProductRelated>();

        [JsonPropertyName("CatalogObjectCloud")]
        public CatalogObjectCloud? CatalogObjectCloud { get; set; }

        [JsonPropertyName("Variants")]
        public List<object> Variants { get; set; } = new List<object>();

        [JsonPropertyName("ProductStory")]
        public List<object> ProductStory { get; set; } = new List<object>();

        [JsonPropertyName("Dictionary")]
        public Dictionary? Dictionary { get; set; }

        [JsonPropertyName("DemoAccount")]
        public bool? DemoAccount { get; set; }
    }

    public partial class CatalogObjectCloud
    {
        [JsonPropertyName("ProductPage")]
        public ProductPage? ProductPage { get; set; }
    }

    public partial class ProductPage
    {
        [JsonPropertyName("QRCodeURL")]
        public Uri? QrCodeUrl { get; set; }

        [JsonPropertyName("URL")]
        public Uri? Url { get; set; }
    }

    public partial class Dictionary
    {
        [JsonPropertyName("model_name")]
        public string? ModelName { get; set; }

        [JsonPropertyName("desc")]
        public string? Desc { get; set; }

        [JsonPropertyName("ean_code")]
        public string? EanCode { get; set; }

        [JsonPropertyName("flash360")]
        public string? Flash360 { get; set; }

        [JsonPropertyName("demo_msg_part3")]
        public string? DemoMsgPart3 { get; set; }

        [JsonPropertyName("repairability_index")]
        public string? RepairabilityIndex { get; set; }

        [JsonPropertyName("zoom_panel_init")]
        public string? ZoomPanelInit { get; set; }

        [JsonPropertyName("cat_name")]
        public string? CatName { get; set; }

        [JsonPropertyName("product_family")]
        public string? ProductFamily { get; set; }

        [JsonPropertyName("back_to_top")]
        public string? BackToTop { get; set; }

        [JsonPropertyName("zoom_panel_out")]
        public string? ZoomPanelOut { get; set; }

        [JsonPropertyName("specs")]
        public string? Specs { get; set; }

        [JsonPropertyName("eu_product_fiche")]
        public string? EuProductFiche { get; set; }

        [JsonPropertyName("reviews_head_name")]
        public string? ReviewsHeadName { get; set; }

        [JsonPropertyName("pdf_specs")]
        public string? PdfSpecs { get; set; }

        [JsonPropertyName("zoom_panel_in")]
        public string? ZoomPanelIn { get; set; }

        [JsonPropertyName("html_content")]
        public string? HtmlContent { get; set; }

        [JsonPropertyName("release_date")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("demo_msg_part1")]
        public string? DemoMsgPart1 { get; set; }

        [JsonPropertyName("demo_insert_desc")]
        public string? DemoInsertDesc { get; set; }

        [JsonPropertyName("pdf_url")]
        public string? PdfUrl { get; set; }

        [JsonPropertyName("prod_code")]
        public string? ProdCode { get; set; }

        [JsonPropertyName("supplier_name")]
        public string? SupplierName { get; set; }

        [JsonPropertyName("video")]
        public string? Video { get; set; }

        [JsonPropertyName("reasons_to_buy")]
        public string? ReasonsToBuy { get; set; }

        [JsonPropertyName("marketing_text")]
        public string? MarketingText { get; set; }

        [JsonPropertyName("options_head_name")]
        public string? OptionsHeadName { get; set; }

        [JsonPropertyName("product_series")]
        public string? ProductSeries { get; set; }

        [JsonPropertyName("demo_msg_part2")]
        public string? DemoMsgPart2 { get; set; }

        [JsonPropertyName("link_integrate_desk")]
        public string? LinkIntegrateDesk { get; set; }

        [JsonPropertyName("zoom_panel_dragg")]
        public string? ZoomPanelDragg { get; set; }

        [JsonPropertyName("eu_energy_label")]
        public string? EuEnergyLabel { get; set; }
    }

    public partial class FeaturesGroup
    {
        [JsonPropertyName("ID")]
        public long? Id { get; set; }

        [JsonPropertyName("SortNo")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SortNo { get; set; }

        [JsonPropertyName("FeatureGroup")]
        public FeatureGroup? FeatureGroup { get; set; }

        [JsonPropertyName("Features")]
        public List<FeatureElement> Features { get; set; } = new List<FeatureElement>();
    }

    public partial class FeatureGroup
    {
        [JsonPropertyName("ID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Id { get; set; }

        [JsonPropertyName("Name")]
        public Name? Name { get; set; }
    }

    public partial class Name
    {
        [JsonPropertyName("Value")]
        public string? Value { get; set; }

        [JsonPropertyName("Language")]
        public Language? Language { get; set; }
    }

    public partial class FeatureElement
    {
        [JsonPropertyName("Localized")]
        public long? Localized { get; set; }

        [JsonPropertyName("ID")]
        public long? Id { get; set; }

        [JsonPropertyName("LocalID")]
        public long? LocalId { get; set; }

        [JsonPropertyName("Type")]
        public FeatureType? Type { get; set; }

        [JsonPropertyName("Value")]
        public string? Value { get; set; }

        [JsonPropertyName("CategoryFeatureId")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? CategoryFeatureId { get; set; }

        [JsonPropertyName("CategoryFeatureGroupID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? CategoryFeatureGroupId { get; set; }

        [JsonPropertyName("SortNo")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SortNo { get; set; }

        [JsonPropertyName("PresentationValue")]
        public string? PresentationValue { get; set; }

        [JsonPropertyName("RawValue")]
        public string? RawValue { get; set; }

        [JsonPropertyName("LocalValue")]
        public string? LocalValue { get; set; }

        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("Mandatory")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Mandatory { get; set; }

        [JsonPropertyName("Searchable")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Searchable { get; set; }

        [JsonPropertyName("Optional")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Optional { get; set; }

        [JsonPropertyName("Feature")]
        public FeatureFeature? Feature { get; set; }
    }

    public partial class FeatureFeature
    {
        [JsonPropertyName("ID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Id { get; set; }

        [JsonPropertyName("Sign")]
        public string? Sign { get; set; }

        [JsonPropertyName("Measure")]
        public Measure? Measure { get; set; }

        [JsonPropertyName("Name")]
        public Name? Name { get; set; }
    }

    public partial class Measure
    {
        [JsonPropertyName("ID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Id { get; set; }

        [JsonPropertyName("Sign")]
        public string? Sign { get; set; }

        [JsonPropertyName("Signs")]
        public Signs? Signs { get; set; }
    }

    public partial class Signs
    {
        [JsonPropertyName("ID")]
        public string? Id { get; set; }

        [JsonPropertyName("_")]
        public string? Empty { get; set; }

        [JsonPropertyName("Language")]
        public Language? Language { get; set; }
    }

    public partial class Gallery
    {
        [JsonPropertyName("ID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Id { get; set; }

        [JsonPropertyName("LowPic")]
        public Uri? LowPic { get; set; }

        [JsonPropertyName("LowSize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? LowSize { get; set; }

        [JsonPropertyName("LowHeight")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? LowHeight { get; set; }

        [JsonPropertyName("LowWidth")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? LowWidth { get; set; }

        [JsonPropertyName("ThumbPic")]
        public Uri? ThumbPic { get; set; }

        [JsonPropertyName("ThumbPicSize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ThumbPicSize { get; set; }

        [JsonPropertyName("Pic")]
        public Uri? Pic { get; set; }

        [JsonPropertyName("Size")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Size { get; set; }

        [JsonPropertyName("PicHeight")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? PicHeight { get; set; }

        [JsonPropertyName("PicWidth")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? PicWidth { get; set; }

        [JsonPropertyName("Pic500x500")]
        public Uri? Pic500X500 { get; set; }

        [JsonPropertyName("Pic500x500Size")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Pic500X500Size { get; set; }

        [JsonPropertyName("Pic500x500Height")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Pic500X500Height { get; set; }

        [JsonPropertyName("Pic500x500Width")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Pic500X500Width { get; set; }

        [JsonPropertyName("No")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? No { get; set; }

        [JsonPropertyName("IsMain")]
        public IsMain? IsMain { get; set; }

        [JsonPropertyName("Updated")]
        public DateTimeOffset? Updated { get; set; }

        [JsonPropertyName("IsPrivate")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? IsPrivate { get; set; }

        [JsonPropertyName("Type")]
        public GalleryType? Type { get; set; }

        [JsonPropertyName("Attributes")]
        public Attributes? Attributes { get; set; }
    }

    public partial class Attributes
    {
        [JsonPropertyName("OriginalFileName")]
        public string? OriginalFileName { get; set; }
    }

    public partial class GeneralInfo
    {
        [JsonPropertyName("IcecatId")]
        public long? IcecatId { get; set; }

        [JsonPropertyName("ReleaseDate")]
        public string? ReleaseDate { get; set; }

        [JsonPropertyName("EndOfLifeDate")]
        public string? EndOfLifeDate { get; set; }

        [JsonPropertyName("Title")]
        public string? Title { get; set; }

        [JsonPropertyName("TitleInfo")]
        public TitleInfo? TitleInfo { get; set; }

        [JsonPropertyName("Brand")]
        public string? Brand { get; set; }

        [JsonPropertyName("BrandID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? BrandId { get; set; }

        [JsonPropertyName("BrandLogo")]
        public Uri? BrandLogo { get; set; }

        [JsonPropertyName("BrandInfo")]
        public BrandInfo? BrandInfo { get; set; }

        [JsonPropertyName("ProductName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("ProductNameInfo")]
        public ProductNameInfo? ProductNameInfo { get; set; }

        [JsonPropertyName("BrandPartCode")]
        public string? BrandPartCode { get; set; }

        [JsonPropertyName("GTIN")]
        public List<string> Gtin { get; set; } = new List<string>();

        [JsonPropertyName("Category")]
        public Category? Category { get; set; }

        [JsonPropertyName("ProductFamily")]
        public BulletPoints? ProductFamily { get; set; }

        [JsonPropertyName("ProductSeries")]
        public ProductSeries? ProductSeries { get; set; }

        [JsonPropertyName("Description")]
        public Description? Description { get; set; }

        [JsonPropertyName("SummaryDescription")]
        public SummaryDescription? SummaryDescription { get; set; }

        [JsonPropertyName("BulletPoints")]
        public BulletPoints? BulletPoints { get; set; }

        [JsonPropertyName("GeneratedBulletPoints")]
        public BulletPoints? GeneratedBulletPoints { get; set; }

        [JsonPropertyName("GTINs")]
        public List<Gtin> GtiNs { get; set; } = new List<Gtin>();
    }

    public partial class BrandInfo
    {
        [JsonPropertyName("BrandName")]
        public string? BrandName { get; set; }

        [JsonPropertyName("BrandLocalName")]
        public string? BrandLocalName { get; set; }

        [JsonPropertyName("BrandLogo")]
        public Uri? BrandLogo { get; set; }
    }

    public partial class BulletPoints
    {
    }

    public partial class Category
    {
        [JsonPropertyName("CategoryID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? CategoryId { get; set; }

        [JsonPropertyName("Name")]
        public Name? Name { get; set; }
    }

    public partial class Gtin
    {
        [JsonPropertyName("GTIN")]
        public string? GtinGtin { get; set; }

        [JsonPropertyName("IsApproved")]
        public bool? IsApproved { get; set; }
    }

    public partial class ProductNameInfo
    {
        [JsonPropertyName("ProductIntName")]
        public string? ProductIntName { get; set; }

        [JsonPropertyName("ProductLocalName")]
        public Name? ProductLocalName { get; set; }
    }

    public partial class ProductSeries
    {
        [JsonPropertyName("SeriesID")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? SeriesId { get; set; }
    }

    public partial class Description
    {
        [JsonPropertyName("LongDesc")]
        public string? LongDesc { get; set; }
    }

    public partial class SummaryDescription
    {
        [JsonPropertyName("ShortSummaryDescription")]
        public string? ShortSummaryDescription { get; set; }

        [JsonPropertyName("LongSummaryDescription")]
        public string? LongSummaryDescription { get; set; }
    }

    public partial class TitleInfo
    {
        [JsonPropertyName("GeneratedIntTitle")]
        public string? GeneratedIntTitle { get; set; }

        [JsonPropertyName("GeneratedLocalTitle")]
        public Name? GeneratedLocalTitle { get; set; }

        [JsonPropertyName("BrandLocalTitle")]
        public Name? BrandLocalTitle { get; set; }
    }

    public partial class Image
    {
        [JsonPropertyName("HighPic")]
        public Uri? HighPic { get; set; }

        [JsonPropertyName("HighPicSize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? HighPicSize { get; set; }

        [JsonPropertyName("HighPicHeight")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? HighPicHeight { get; set; }

        [JsonPropertyName("HighPicWidth")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? HighPicWidth { get; set; }

        [JsonPropertyName("LowPic")]
        public Uri? LowPic { get; set; }

        [JsonPropertyName("LowPicSize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? LowPicSize { get; set; }

        [JsonPropertyName("LowPicHeight")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? LowPicHeight { get; set; }

        [JsonPropertyName("LowPicWidth")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? LowPicWidth { get; set; }

        [JsonPropertyName("Pic500x500")]
        public Uri? Pic500X500 { get; set; }

        [JsonPropertyName("Pic500x500Size")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Pic500X500Size { get; set; }

        [JsonPropertyName("Pic500x500Height")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Pic500X500Height { get; set; }

        [JsonPropertyName("Pic500x500Width")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Pic500X500Width { get; set; }

        [JsonPropertyName("ThumbPic")]
        public Uri? ThumbPic { get; set; }

        [JsonPropertyName("ThumbPicSize")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? ThumbPicSize { get; set; }
    }

    public partial class Multimedia
    {
        [JsonPropertyName("ID")]
        public string? Id { get; set; }

        [JsonPropertyName("URL")]
        public Uri? Url { get; set; }

        [JsonPropertyName("Type")]
        public string? Type { get; set; }

        [JsonPropertyName("ContentType")]
        public string? ContentType { get; set; }

        [JsonPropertyName("KeepAsUrl")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? KeepAsUrl { get; set; }

        [JsonPropertyName("Description")]
        public string? Description { get; set; }

        [JsonPropertyName("Size")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? Size { get; set; }

        [JsonPropertyName("IsPrivate")]
        [JsonConverter(typeof(ParseStringConverter))]
        public long? IsPrivate { get; set; }

        [JsonPropertyName("Updated")]
        public DateTimeOffset? Updated { get; set; }

        [JsonPropertyName("Language")]
        public string? Language { get; set; }

        [JsonPropertyName("IsVideo")]
        public long? IsVideo { get; set; }

        [JsonPropertyName("ThumbUrl")]
        public Uri? ThumbUrl { get; set; }

        [JsonPropertyName("PreviewUrl")]
        public Uri? PreviewUrl { get; set; }
    }

    public partial class ProductRelated
    {
        [JsonPropertyName("ID")]
        public long? Id { get; set; }

        [JsonPropertyName("CategoryID")]
        public long? CategoryId { get; set; }

        [JsonPropertyName("Preferred")]
        public long? Preferred { get; set; }

        [JsonPropertyName("IcecatID")]
        public long? IcecatId { get; set; }

        [JsonPropertyName("ProductCode")]
        public string? ProductCode { get; set; }

        [JsonPropertyName("ThumbPic")]
        public string? ThumbPic { get; set; }

        [JsonPropertyName("ProductName")]
        public string? ProductName { get; set; }

        [JsonPropertyName("Brand")]
        public string? Brand { get; set; }

        [JsonPropertyName("BrandID")]
        public long? BrandId { get; set; }

        [JsonPropertyName("ProductRelatedLocales")]
        public List<ProductRelatedLocale> ProductRelatedLocales { get; set; } = new List<ProductRelatedLocale>();
    }

    public partial class ProductRelatedLocale
    {
        [JsonPropertyName("ID")]
        public long? Id { get; set; }

        [JsonPropertyName("Language")]
        public Language? Language { get; set; }

        [JsonPropertyName("Preferred")]
        public long? Preferred { get; set; }

        [JsonPropertyName("StartDate")]
        public string? StartDate { get; set; }

        [JsonPropertyName("EndDate")]
        public string? EndDate { get; set; }
    }

    public enum Language { Null, En, Ru, Uk };

    public enum FeatureType { Dropdown, MultiDropdown, Numerical, YN };

    public enum IsMain { N, Y };

    public enum GalleryType { ProductImage };
    
    internal static class Converter
    {
        public static readonly JsonSerializerOptions Settings = new(JsonSerializerDefaults.General)
        {
            Converters =
            {
                LanguageConverter.Singleton,
                FeatureTypeConverter.Singleton,
                IsMainConverter.Singleton,
                GalleryTypeConverter.Singleton,
                new DateOnlyConverter(),
                new TimeOnlyConverter(),
                IsoDateTimeOffsetConverter.Singleton
            },
        };
    }

    internal class ParseStringConverter : JsonConverter<long>
    {
        public override bool CanConvert(Type t) => t == typeof(long);

        public override long Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            long l;
            if (Int64.TryParse(value, out l))
            {
                return l;
            }
            throw new Exception("Cannot unmarshal type long");
        }

        public override void Write(Utf8JsonWriter writer, long value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value.ToString(), options);
            return;
        }

        public static readonly ParseStringConverter Singleton = new ParseStringConverter();
    }

    internal class LanguageConverter : JsonConverter<Language>
    {
        public override bool CanConvert(Type t) => t == typeof(Language);

        public override Language Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "EN":
                    return Language.En;
                case "RU":
                    return Language.Ru;
                case "UK":
                    return Language.Uk;
                case "":
                    return Language.Null;
                case null:
                    return Language.Null;
            }
            throw new Exception("Cannot unmarshal type Language");
        }

        public override void Write(Utf8JsonWriter writer, Language value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case Language.En:
                    JsonSerializer.Serialize(writer, "EN", options);
                    return;
                case Language.Ru:
                    JsonSerializer.Serialize(writer, "RU", options);
                    return;
                case Language.Uk:
                    JsonSerializer.Serialize(writer, "UK", options);
                    return;
                case Language.Null:
                    JsonSerializer.Serialize(writer, "", options);
                    return;
            }
            throw new Exception("Cannot marshal type Language");
        }

        public static readonly LanguageConverter Singleton = new LanguageConverter();
    }

    internal class FeatureTypeConverter : JsonConverter<FeatureType>
    {
        public override bool CanConvert(Type t) => t == typeof(FeatureType);

        public override FeatureType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "dropdown":
                    return FeatureType.Dropdown;
                case "multi_dropdown":
                    return FeatureType.MultiDropdown;
                case "numerical":
                    return FeatureType.Numerical;
                case "y_n":
                    return FeatureType.YN;
            }
            throw new Exception("Cannot unmarshal type FeatureType");
        }

        public override void Write(Utf8JsonWriter writer, FeatureType value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case FeatureType.Dropdown:
                    JsonSerializer.Serialize(writer, "dropdown", options);
                    return;
                case FeatureType.MultiDropdown:
                    JsonSerializer.Serialize(writer, "multi_dropdown", options);
                    return;
                case FeatureType.Numerical:
                    JsonSerializer.Serialize(writer, "numerical", options);
                    return;
                case FeatureType.YN:
                    JsonSerializer.Serialize(writer, "y_n", options);
                    return;
            }
            throw new Exception("Cannot marshal type FeatureType");
        }

        public static readonly FeatureTypeConverter Singleton = new FeatureTypeConverter();
    }

    internal class IsMainConverter : JsonConverter<IsMain>
    {
        public override bool CanConvert(Type t) => t == typeof(IsMain);

        public override IsMain Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            switch (value)
            {
                case "N":
                    return IsMain.N;
                case "Y":
                    return IsMain.Y;
            }
            throw new Exception("Cannot unmarshal type IsMain");
        }

        public override void Write(Utf8JsonWriter writer, IsMain value, JsonSerializerOptions options)
        {
            switch (value)
            {
                case IsMain.N:
                    JsonSerializer.Serialize(writer, "N", options);
                    return;
                case IsMain.Y:
                    JsonSerializer.Serialize(writer, "Y", options);
                    return;
            }
            throw new Exception("Cannot marshal type IsMain");
        }

        public static readonly IsMainConverter Singleton = new IsMainConverter();
    }

    internal class GalleryTypeConverter : JsonConverter<GalleryType>
    {
        public override bool CanConvert(Type t) => t == typeof(GalleryType);

        public override GalleryType Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            if (value == "ProductImage")
            {
                return GalleryType.ProductImage;
            }
            throw new Exception("Cannot unmarshal type GalleryType");
        }

        public override void Write(Utf8JsonWriter writer, GalleryType value, JsonSerializerOptions options)
        {
            if (value == GalleryType.ProductImage)
            {
                JsonSerializer.Serialize(writer, "ProductImage", options);
                return;
            }
            throw new Exception("Cannot marshal type GalleryType");
        }

        public static readonly GalleryTypeConverter Singleton = new GalleryTypeConverter();
    }

    public class DateOnlyConverter : JsonConverter<DateOnly>
    {
        private readonly string serializationFormat;
        public DateOnlyConverter() : this(null) { }

        public DateOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "yyyy-MM-dd";
        }

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return DateOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    public class TimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private readonly string serializationFormat;

        public TimeOnlyConverter() : this(null) { }

        public TimeOnlyConverter(string? serializationFormat)
        {
            this.serializationFormat = serializationFormat ?? "HH:mm:ss.fff";
        }

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var value = reader.GetString();
            return TimeOnly.Parse(value!);
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
                => writer.WriteStringValue(value.ToString(serializationFormat));
    }

    internal class IsoDateTimeOffsetConverter : JsonConverter<DateTimeOffset>
    {
        public override bool CanConvert(Type t) => t == typeof(DateTimeOffset);

        private const string DefaultDateTimeFormat = "yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK";

        private DateTimeStyles _dateTimeStyles = DateTimeStyles.RoundtripKind;
        private string? _dateTimeFormat;
        private CultureInfo? _culture;

        public DateTimeStyles DateTimeStyles
        {
            get => _dateTimeStyles;
            set => _dateTimeStyles = value;
        }

        public string? DateTimeFormat
        {
            get => _dateTimeFormat ?? string.Empty;
            set => _dateTimeFormat = (string.IsNullOrEmpty(value)) ? null : value;
        }

        public CultureInfo Culture
        {
            get => _culture ?? CultureInfo.CurrentCulture;
            set => _culture = value;
        }

        public override void Write(Utf8JsonWriter writer, DateTimeOffset value, JsonSerializerOptions options)
        {
            string text;


            if ((_dateTimeStyles & DateTimeStyles.AdjustToUniversal) == DateTimeStyles.AdjustToUniversal
                    || (_dateTimeStyles & DateTimeStyles.AssumeUniversal) == DateTimeStyles.AssumeUniversal)
            {
                value = value.ToUniversalTime();
            }

            text = value.ToString(_dateTimeFormat ?? DefaultDateTimeFormat, Culture);

            writer.WriteStringValue(text);
        }

        public override DateTimeOffset Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string? dateText = reader.GetString();

            if (string.IsNullOrEmpty(dateText) == false)
            {
                if (!string.IsNullOrEmpty(_dateTimeFormat))
                {
                    return DateTimeOffset.ParseExact(dateText, _dateTimeFormat, Culture, _dateTimeStyles);
                }
                else
                {
                    return DateTimeOffset.Parse(dateText, Culture, _dateTimeStyles);
                }
            }
            else
            {
                return default(DateTimeOffset);
            }
        }


        public static readonly IsoDateTimeOffsetConverter Singleton = new IsoDateTimeOffsetConverter();
    }
}
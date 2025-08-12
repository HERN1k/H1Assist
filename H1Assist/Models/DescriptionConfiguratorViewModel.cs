namespace H1Assist.Models
{
    public sealed class DescriptionConfiguratorViewModel
    {
        public string CleanedDescription { get; set; } = string.Empty;
        public string CleanDescriptionHtml { get; set; } = string.Empty;
        public List<string> ExternalImages { get; set; } = new List<string>();
    }
}
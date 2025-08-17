namespace H1Assist.Models
{
    public sealed class DescriptionConfiguratorViewModel
    {
        public string CleanedDescription { get; set; } = string.Empty;
        public string CleanDescriptionHtml { get; set; } = string.Empty;
        public Dictionary<string, string> ExternalImages { get; set; } = new Dictionary<string, string>();
        public List<string> ExternalServiceOptions { get; set; } = new List<string>();
        public string DownloadLinks { get; set; } = string.Empty;
    }
}
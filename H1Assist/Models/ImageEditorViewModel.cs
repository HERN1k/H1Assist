using Microsoft.AspNetCore.Mvc.Rendering;

namespace H1Assist.Models
{
    public sealed class ImageEditorViewModel
    {
        public List<SelectListItem>? ValidExtensionsOptions { get; set; }
        public string ValidExtensions { get; set; } = string.Empty;
    }
}
using Microsoft.AspNetCore.Mvc;

namespace H1Assist.ViewComponents
{
    public sealed class ContentTextItemViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string id, string headline, string? text, string marginBottom)
        {
            return View(new ContentTextItemViewComponentProps
            {
                ID = id,
                Headline = headline,
                Text = text ?? string.Empty,
                MarginBottom = marginBottom
            });
        }
    }

    internal record ContentTextItemViewComponentProps
    {
        public string ID
        {
            get => _id;
            set => _id = value?.Trim() ?? string.Empty;
        }
        public string Headline
        {
            get => _headline;
            set => _headline = value?.Trim() ?? string.Empty;
        }
        public string Text 
        { 
            get => _text; 
            set => _text = value?.Trim() ?? string.Empty; 
        }
        public string MarginBottom
        { 
            get => _marginBottom; 
            set => _marginBottom = value?.Trim() ?? "mb-2"; 
        }

        private string _id = string.Empty;
        private string _headline = string.Empty;
        private string _text = string.Empty;
        private string _marginBottom = "mb-2";
    }
}
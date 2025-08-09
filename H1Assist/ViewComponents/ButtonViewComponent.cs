using Microsoft.AspNetCore.Mvc;

namespace H1Assist.ViewComponents
{
    public sealed class ButtonViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(string text, string type, string onClick, string sizeClasses, string moreClasses, string wrapperClasses)
        {
            return View(new ButtonViewComponentProps
            {
                Type = type,
                Text = text,
                OnClick = onClick,
                SizeClasses = sizeClasses,
                MoreClasses = moreClasses,
                WrapperClasses = wrapperClasses
            });
        }
    }

    internal record ButtonViewComponentProps
    {
        public string Type 
        { 
            get => _type;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _type = "Button";
                }
                else
                {
                    _type = value.Trim();
                } 
            }
        } 
        public string Text 
        { 
            get => _text;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _text = "button";
                }
                else
                {
                    _text = value.Trim();
                }
            }
        }
        public string OnClick
        {
            get => _onClick;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _onClick = string.Empty;
                }
                else
                {
                    _onClick = value.Trim();
                }
            }
        }
        public string SizeClasses
        {
            get => _sizeClasses;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _sizeClasses = "py-2 px-4";
                }
                else
                {
                    _sizeClasses = value.Trim();
                }
            }
        }
        public string MoreClasses
        {
            get => _moreClasses;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _moreClasses = string.Empty;
                }
                else
                {
                    _moreClasses = value.Trim();
                }
            }
        }
        public string WrapperClasses
        {
            get => _wrapperClasses;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    _wrapperClasses = string.Empty;
                }
                else
                {
                    _wrapperClasses = value.Trim();
                }
            }
        }

        private string _type = "Button";
        private string _text = "button";
        private string _onClick = string.Empty;
        private string _sizeClasses = "py-2 px-4";
        private string _moreClasses = string.Empty;
        private string _wrapperClasses = string.Empty;
    }
}
using Application.Interfaces;
using Domain.ValueObjects;
using H1Assist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace H1Assist.Controllers
{
    public class ImageEditorController : Controller
    {
        private readonly IImageEditorService _imageEditor;
        private readonly IHtmlLocalizer<SharedResource> _localizer;

        public ImageEditorController(IImageEditorService imageEditor, IHtmlLocalizer<SharedResource> localizer)
        {
            this._imageEditor = imageEditor ?? throw new ArgumentNullException(nameof(imageEditor));
            this._localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(CreateModel());
        }

        [HttpPost]
        [ActionName("ConvertImageFormat")]
        public async Task<IActionResult> ConvertImageFormatAsync(List<IFormFile> uploadFiles, string selectedExtension, int selectedWidth)
        {
            if (!ModelState.IsValid || uploadFiles == null || uploadFiles.Count == 0)
            {
                ModelState.AddModelError(string.Empty, _localizer["PLEASE_UPLOAD_AT_LEAST_ONE_PHOTO_KEY"].Value);
                return View("Index", CreateModel());
            }

            if (string.IsNullOrWhiteSpace(selectedExtension) || !ImageExtension.TryParse(selectedExtension, out ImageExtension? outputExtension) || outputExtension == null)
            {
                ModelState.AddModelError(string.Empty, _localizer["INVALID_IMAGE_FORMAT_SELECTED_KEY"].Value);
                return View("Index", CreateModel());
            }

            try
            {
                byte[] result = await _imageEditor.ConvertImageFormatAsync(
                    outputExtension: outputExtension,
                    selectedWidth: selectedWidth,
                    images: uploadFiles.ToArray()
                );

                return File(result, "application/zip", "converted_images.zip");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View("Index", CreateModel());
            }
        }

        private static ImageEditorViewModel CreateModel()
        {
            ImageEditorViewModel model = new ImageEditorViewModel();

            model.ValidExtensionsOptions = ImageExtension.ValidExtensions.Select(ext => new SelectListItem
            {
                Value = ext,
                Text = ext,
                Selected = ext.Equals(ImageExtension.JPG.ToString(), StringComparison.InvariantCultureIgnoreCase)
            }).ToList();

            model.ValidExtensions = string.Join(", ", ImageExtension.ValidExtensions
                .Select(ext => ext.Substring(1).ToUpperInvariant()));

            return model;
        }
    }
}
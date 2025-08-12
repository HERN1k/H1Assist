using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using Application.Interfaces;
using Domain.Helpers;
using H1Assist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace H1Assist.Controllers
{
    public sealed partial class DescriptionConfiguratorController : Controller
    {
        private readonly IHtmlLocalizer<SharedResource> _localizer;
        private readonly IDescriptionService _description;
        private readonly IHtmlManagerService _htmlManager;
        private static readonly Regex _urlRegex = UrlRegex();
        private const string _brainStyles = """
        <style>
        .description-block {
            display: flex;
            flex-direction: column;
            min-height: 200px;
            position: relative;
            width: 100%;
        }
        .background-image {
            order: 2;
            width: 100%;
            margin: 0 auto;
        }
        .description-content {
            order: 1;
            width: 100%;
        }
        .description-title {
            font-size: 21px;
            font-weight: 700;
            line-height: 34px;
            padding: 15px 2% 4px;
            text-align: center;
            width: 100%;
        }
        .description-text {
            color: #000;
            font-size: 16px;
            font-weight: 400;
            line-height: 20px;
            margin-bottom: 14px;
            padding: 4px 5% 0;
            text-align: center;
            width: 100%;
        }
        </style>
        """;

        public DescriptionConfiguratorController(
            IHtmlLocalizer<SharedResource> localizer,
            IDescriptionService description,
            IHtmlManagerService htmlManager
        )
        {
            this._localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this._description = description ?? throw new ArgumentNullException(nameof(description));
            this._htmlManager = htmlManager ?? throw new ArgumentNullException(nameof(htmlManager));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(CreateModel());
        }

        [HttpPost]
        [ActionName("FindDescription")]
        //public async Task<IActionResult> FindDescriptionAsync(string descriptionUrl, string dirUrl, string externalService)
        public async Task<IActionResult> FindDescriptionAsync(string descriptionUrl, string dirUrl)
        {
            //if (!ModelState.IsValid || string.IsNullOrWhiteSpace(descriptionUrl) || string.IsNullOrWhiteSpace(dirUrl) || string.IsNullOrWhiteSpace(externalService))
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(descriptionUrl) || string.IsNullOrWhiteSpace(dirUrl))
            {
                ModelState.AddModelError(string.Empty, _localizer["INVALID_INPUT_DATA_KEY"].Value);
                return View(nameof(Index), CreateModel());
            }

            if (!_urlRegex.IsMatch(descriptionUrl) || !_urlRegex.IsMatch(dirUrl))
            {
                ModelState.AddModelError(string.Empty, _localizer["INVALID_URL_KEY"].Value);
                return View(nameof(Index), CreateModel());
            }

            try
            {
                IDocument document = await _htmlManager.GetDocumentAsync(descriptionUrl);

                IElement? element = document.QuerySelector(".product-additional-description");

                if (element == null)
                {
                    ModelState.AddModelError(string.Empty, _localizer["DESCRIPTION_NOT_FOUND_KEY"].Value);
                    return View(nameof(Index), CreateModel());
                }

                (string cleanDescriptionHtml, List<string> externalImages) = _htmlManager.CleanDescriptionHtml(element, dirUrl);

                if (true)
                {
                    cleanDescriptionHtml = string.Concat(_brainStyles, cleanDescriptionHtml);
                }

                var model = CreateModel();

                model.ExternalImages = externalImages;
                model.CleanDescriptionHtml = cleanDescriptionHtml;

                //TempData.Set(nameof(DescriptionConfiguratorViewModel.ExternalImages), JsonSerializer.Serialize(externalImages));
                //TempData.Set(nameof(DescriptionConfiguratorViewModel.CleanDescriptionHtml), cleanDescriptionHtml);

                return View(nameof(Index), model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(nameof(Index), CreateModel());
            }
        }

        [HttpPost]
        [ActionName("DescriptionClean")]
        public async Task<IActionResult> DescriptionCleanAsync(string formattedHtml)
        {
            if (!ModelState.IsValid)
                return View(nameof(Index), CreateModel());

            if (string.IsNullOrWhiteSpace(formattedHtml))
            {
                return RedirectToAction(nameof(Index), CreateModel());
            }

            string cleanedDescription = await _htmlManager.DescriptionClean(formattedHtml);

            TempData.Set(nameof(DescriptionConfiguratorViewModel.CleanedDescription), cleanedDescription);

            return RedirectToAction(nameof(Index));
        }

        private DescriptionConfiguratorViewModel CreateModel()
        {
            DescriptionConfiguratorViewModel model = new DescriptionConfiguratorViewModel();

            //model.CleanedDescription = TempData.Get(nameof(DescriptionConfiguratorViewModel.CleanedDescription), model.CleanedDescription);
            //model.CleanDescriptionHtml = TempData.Get(nameof(DescriptionConfiguratorViewModel.CleanDescriptionHtml), model.CleanDescriptionHtml);
            //model.ExternalImages = TempData.Get(
            //    key: nameof(DescriptionConfiguratorViewModel.ExternalImages),
            //    fallback: model.ExternalImages,
            //    func: value => value is string valueStr ? JsonSerializer.Deserialize<List<string>>(valueStr) : null
            //);
            
            return model;
        }

        [GeneratedRegex(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-._~:\/?#[\]@!$&'()*+,;=%]*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex UrlRegex();
    }
}
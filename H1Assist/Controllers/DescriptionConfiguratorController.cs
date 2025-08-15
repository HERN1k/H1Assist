using System.Text.Json;
using System.Text.RegularExpressions;

using AngleSharp.Dom;

using Application.Interfaces;

using Domain.Helpers;
using Domain.ValueObjects;

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
        public async Task<IActionResult> FindDescriptionAsync(string descriptionUrl, string? dirUrl = "")
        {
            //if (!ModelState.IsValid || string.IsNullOrWhiteSpace(descriptionUrl) || string.IsNullOrWhiteSpace(dirUrl) || string.IsNullOrWhiteSpace(externalService))
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(descriptionUrl))
            {
                ModelState.AddModelError(string.Empty, _localizer["INVALID_INPUT_DATA_KEY"].Value);
                return View(nameof(Index), CreateModel());
            }

            if (!_urlRegex.IsMatch(descriptionUrl))
            {
                ModelState.AddModelError(string.Empty, _localizer["INVALID_URL_KEY"].Value);
                return View(nameof(Index), CreateModel());
            }

            try
            {
                IDocument document = await _htmlManager.GetDocumentAsync(descriptionUrl);

                IElement? element = document.QuerySelectorAll(".p-description__content").ElementAtOrDefault(0);

                if (element == null)
                {
                    ModelState.AddModelError(string.Empty, _localizer["DESCRIPTION_NOT_FOUND_KEY"].Value);
                    return View(nameof(Index), CreateModel());
                }

                (
                    string cleanDescriptionHtml,
                    Dictionary<string, string> externalImages
                ) = await _htmlManager.CleanDescriptionHtmlAsync(element, ExternalService.Allo, dirUrl);

                DescriptionConfiguratorViewModel model = CreateModel();

                model.ExternalImages = externalImages;
                model.CleanDescriptionHtml = cleanDescriptionHtml;

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
                return View(nameof(Index), CreateModel());
            }

            DescriptionConfiguratorViewModel model = CreateModel();

            model.CleanedDescription = await _htmlManager.DescriptionClean(formattedHtml);

            return View(nameof(Index), model);
        }

        private static DescriptionConfiguratorViewModel CreateModel()
        {
            DescriptionConfiguratorViewModel model = new DescriptionConfiguratorViewModel();

            return model;
        }

        [GeneratedRegex(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-._~:\/?#[\]@!$&'()*+,;=%]*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex UrlRegex();
    }
}
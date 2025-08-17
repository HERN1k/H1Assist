using System.Text.Json;
using System.Text.RegularExpressions;
using Application.DTOs;
using Application.Interfaces;
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
        public async Task<IActionResult> FindDescriptionAsync(string descriptionUrl, string externalService, string? dirUrl = null)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(descriptionUrl) || string.IsNullOrWhiteSpace(externalService))
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
                DescriptionConfiguratorViewModel model = CreateModel();

                if (!Enum.TryParse<ExternalService>(externalService, true, out ExternalService service))
                {
                    ModelState.AddModelError(string.Empty, "INVALID_EXTERNAL_SERVICE_KEY");
                    return View(nameof(Index), model);
                }

                CleanDescriptionHtmlDto? description = await _description.CleanHtmlAsync(descriptionUrl, service, dirUrl, false);

                if (description == null)
                {
                    ModelState.AddModelError(string.Empty, _localizer["DESCRIPTION_NOT_FOUND_KEY"].Value);
                    return View(nameof(Index), CreateModel());
                }

                model.ExternalImages = description.ExternalImages;
                model.CleanDescriptionHtml = description.Value;
                model.DownloadLinks = JsonSerializer.Serialize<Dictionary<string, string>>(description.ExternalImages);

                return View(nameof(Index), model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return View(nameof(Index), CreateModel());
            }
        }

        [HttpPost]
        [ActionName("DownloadMedia")]
        public async Task<IActionResult> DownloadMediaAsync(string linksArr)
        {
            if (!ModelState.IsValid || string.IsNullOrWhiteSpace(linksArr))
            {
                ModelState.AddModelError(string.Empty, _localizer["INVALID_INPUT_DATA_KEY"].Value);
                return View(nameof(Index), CreateModel());
            }

            try
            {
                Dictionary<string, string> links = JsonSerializer.Deserialize<Dictionary<string, string>>(linksArr) 
                    ?? new Dictionary<string, string>();

                if (links.Count == 0)
                {
                    return Ok();
                }

                Dictionary<string, string> base64Media = links
                    .Where(link => link.Key.StartsWith(
                        "data:image/jpg;base64", 
                        StringComparison.InvariantCultureIgnoreCase
                    ))
                    .ToDictionary();
                string[] media = links
                    .Where(link => !link.Key.StartsWith(
                        "data:image/jpg;base64",
                        StringComparison.InvariantCultureIgnoreCase
                    ))
                    .Select(link => link.Key)
                    .ToArray();

                byte[] result = await _description.DownloadMediaAsync(media, base64Media);

                return File(result, "application/zip", "downloaded_images.zip");
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

            model.ExternalServiceOptions = Enum
                .GetNames<ExternalService>()
                .Where(service => !service.Equals(ExternalService.EKatalog.ToString(), StringComparison.InvariantCultureIgnoreCase))
                .ToList(); 

            return model;
        }

        [GeneratedRegex(@"^(https?:\/\/)?([\w\-]+\.)+[\w\-]+(\/[\w\-._~:\/?#[\]@!$&'()*+,;=%]*)?$", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
        private static partial Regex UrlRegex();
    }
} 
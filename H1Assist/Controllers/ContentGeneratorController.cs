using Application.Interfaces;
using Domain.ValueObjects;
using H1Assist.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Localization;

namespace H1Assist.Controllers
{
    public class ContentGeneratorController : Controller
    {
        private readonly ISeoGeneratorService _seoGenerator;
        private readonly IHtmlLocalizer<SharedResource> _localizer;
        private readonly IDescriptionService _description;

        public ContentGeneratorController(
            ISeoGeneratorService seoGenerator, 
            IHtmlLocalizer<SharedResource> localizer,
            IDescriptionService description
        )
        {
            this._seoGenerator = seoGenerator ?? throw new ArgumentNullException(nameof(seoGenerator));
            this._localizer = localizer ?? throw new ArgumentNullException(nameof(localizer));
            this._description = description ?? throw new ArgumentNullException(nameof(description));
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(CreateModel());
        }

        [HttpPost]
        [ActionName("Generate")]
        public async Task<IActionResult> GenerateSeoContent(string productNameUA, string productNameRU)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, _localizer["INVALID_INPUT_DATA_KEY"].Value);
                return View("Index");
            }

            if (string.IsNullOrWhiteSpace(productNameUA) && string.IsNullOrWhiteSpace(productNameRU))
            {
                ModelState.AddModelError(string.Empty, _localizer["PLEASE_PROVIDE_AT_LEAST_ONE_PRODUCT_NAME_KEY"].Value);
                return View("Index");
            }

            try
            {
                productNameUA = productNameUA.Trim();
                productNameRU = productNameRU.Trim();

                ContentGeneratorViewModel model = CreateModel();

                model.ContentVisible = true;

                model.ProductNameUA = productNameUA;
                model.HeadingUA = _seoGenerator.GenerateHeading(productNameUA);
                model.TitleUA = _seoGenerator.GenerateTitle(productNameUA);
                model.KeywordsUA = _seoGenerator.GenerateKeywords(productNameUA);
                model.DescriptionUA = _seoGenerator.GenerateDescription(productNameUA);

                model.ProductNameRU = productNameRU;
                model.HeadingRU = _seoGenerator.GenerateHeading(productNameRU, Language.RU);
                model.TitleRU = _seoGenerator.GenerateTitle(productNameRU, Language.RU);
                model.KeywordsRU = _seoGenerator.GenerateKeywords(productNameRU, Language.RU);
                model.DescriptionRU = _seoGenerator.GenerateDescription(productNameRU, Language.RU);

                var characteristics = await _description.GenerateAsync(productNameUA, Language.UA);

                model.Characteristics = characteristics.Characteristics;

                return View("Index", model);
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, _localizer["AN_ERROR_OCCURRED_KEY"].Value);
                return View("Index");
            }
        }

        private static ContentGeneratorViewModel CreateModel()
        {
            ContentGeneratorViewModel model = new ContentGeneratorViewModel();

            model.ContentVisible = false;

            return model;
        }
    }
}
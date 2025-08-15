using System.Text.Json;
using Application.Interfaces;
using Domain.Helpers;
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
        public async Task<IActionResult> GenerateContentAsync(string productNameUA, string productNameRU)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, _localizer["INVALID_INPUT_DATA_KEY"].Value);
                return View(nameof(Index), CreateModel());
            }

            if (string.IsNullOrWhiteSpace(productNameUA) && string.IsNullOrWhiteSpace(productNameRU))
            {
                ModelState.AddModelError(string.Empty, _localizer["PLEASE_PROVIDE_AT_LEAST_ONE_PRODUCT_NAME_KEY"].Value);
                return View(nameof(Index), CreateModel());
            }

            try
            {
                productNameUA = productNameUA.Trim();
                productNameRU = productNameRU.Trim();
                List<ProductCharacteristic> characteristics = await _description.GenerateCharacteristicsAsync(productNameUA, Language.UA);

                TempData.Set(nameof(ContentGeneratorViewModel.ContentVisible), true);
                TempData.Set(nameof(ContentGeneratorViewModel.ProductNameUA), productNameUA);
                TempData.Set(nameof(ContentGeneratorViewModel.HeadingUA), _seoGenerator.GenerateHeading(productNameUA));
                TempData.Set(nameof(ContentGeneratorViewModel.TitleUA), _seoGenerator.GenerateTitle(productNameUA));
                TempData.Set(nameof(ContentGeneratorViewModel.KeywordsUA), _seoGenerator.GenerateKeywords(productNameUA));
                TempData.Set(nameof(ContentGeneratorViewModel.DescriptionUA), _seoGenerator.GenerateDescription(productNameUA));
                TempData.Set(nameof(ContentGeneratorViewModel.ProductNameRU), productNameRU);
                TempData.Set(nameof(ContentGeneratorViewModel.HeadingRU), _seoGenerator.GenerateHeading(productNameRU, Language.RU));
                TempData.Set(nameof(ContentGeneratorViewModel.TitleRU), _seoGenerator.GenerateTitle(productNameRU, Language.RU));
                TempData.Set(nameof(ContentGeneratorViewModel.KeywordsRU), _seoGenerator.GenerateKeywords(productNameRU, Language.RU));
                TempData.Set(nameof(ContentGeneratorViewModel.DescriptionRU), _seoGenerator.GenerateDescription(productNameRU, Language.RU));
                TempData.Set(nameof(ContentGeneratorViewModel.Characteristics), JsonSerializer.Serialize(characteristics));

                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, _localizer["AN_ERROR_OCCURRED_KEY"].Value);
                return View(nameof(Index), CreateModel());
            }
        }

        private ContentGeneratorViewModel CreateModel()
        {
            ContentGeneratorViewModel model = new ContentGeneratorViewModel();

            model.ContentVisible = false;

            model.ContentVisible    = TempData.Get(nameof(ContentGeneratorViewModel.ContentVisible), model.ContentVisible);
            model.ProductNameUA     = TempData.Get(nameof(ContentGeneratorViewModel.ProductNameUA), model.ProductNameUA);
            model.HeadingUA         = TempData.Get(nameof(ContentGeneratorViewModel.HeadingUA), model.HeadingUA);
            model.TitleUA           = TempData.Get(nameof(ContentGeneratorViewModel.TitleUA), model.TitleUA);
            model.KeywordsUA        = TempData.Get(nameof(ContentGeneratorViewModel.KeywordsUA), model.KeywordsUA);
            model.DescriptionUA     = TempData.Get(nameof(ContentGeneratorViewModel.DescriptionUA), model.DescriptionUA);
            model.ProductNameRU     = TempData.Get(nameof(ContentGeneratorViewModel.ProductNameRU), model.ProductNameRU);
            model.HeadingRU         = TempData.Get(nameof(ContentGeneratorViewModel.HeadingRU), model.HeadingRU);
            model.TitleRU           = TempData.Get(nameof(ContentGeneratorViewModel.TitleRU), model.TitleRU);
            model.KeywordsRU        = TempData.Get(nameof(ContentGeneratorViewModel.KeywordsRU), model.KeywordsRU);
            model.DescriptionRU     = TempData.Get(nameof(ContentGeneratorViewModel.DescriptionRU), model.DescriptionRU);
            model.Characteristics   = TempData.Get(
                key: nameof(ContentGeneratorViewModel.Characteristics),
                fallback: model.Characteristics,
                func: value => value is string valueStr ? JsonSerializer.Deserialize<List<ProductCharacteristic>>(valueStr) : null
            );

            return model;
        }
    }
}
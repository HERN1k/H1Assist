using Application.Interfaces;
using Domain.ValueObjects;
using H1Assist.Models;
using Microsoft.AspNetCore.Mvc;

namespace H1Assist.Controllers
{
    public class ContentGeneratorController : Controller
    {
        private readonly ISeoGeneratorService _seoGenerator;

        public ContentGeneratorController(ISeoGeneratorService seoGenerator)
        {
            this._seoGenerator = seoGenerator ?? throw new ArgumentNullException(nameof(seoGenerator));
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ContentVisible = false;

            return View(CreateModel());
        }

        [HttpPost]
        [ActionName("Generate")]
        public IActionResult GenerateSeoContent(string productNameUA, string productNameRU)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Invalid input data.");
                return View("Index");
            }

            if (string.IsNullOrWhiteSpace(productNameUA) && string.IsNullOrWhiteSpace(productNameRU))
            {
                ModelState.AddModelError(string.Empty, "Please provide at least one product name.");
                return View("Index");
            }

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

            return View("Index", model);
        }

        private static ContentGeneratorViewModel CreateModel()
        {
            ContentGeneratorViewModel model = new ContentGeneratorViewModel();

            model.ContentVisible = false;

            return model;
        }
    }
}
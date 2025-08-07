using Application.Interfaces;

using Domain.ValueObjects;

using Microsoft.AspNetCore.Mvc;

namespace H1Assist.Controllers
{
    public class SeoContentController : Controller
    {
        private readonly ISeoGeneratorService _seoGenerator;

        public SeoContentController(ISeoGeneratorService seoGenerator)
        {
            this._seoGenerator = seoGenerator ?? throw new ArgumentNullException(nameof(seoGenerator));
        }

        [HttpGet]
        public IActionResult Index()
        {
            ViewBag.ContentVisible = false;

            return View();
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

            ViewBag.ContentVisible = true;

            ViewBag.ProductNameUA = productNameUA;
            ViewBag.HeadingUA = _seoGenerator.GenerateHeading(productNameUA);
            ViewBag.TitleUA = _seoGenerator.GenerateTitle(productNameUA);
            ViewBag.KeywordsUA = _seoGenerator.GenerateKeywords(productNameUA);
            ViewBag.DescriptionUA = _seoGenerator.GenerateDescription(productNameUA);

            ViewBag.ProductNameRU = productNameRU;
            ViewBag.HeadingRU = _seoGenerator.GenerateHeading(productNameRU, Language.RU);
            ViewBag.TitleRU = _seoGenerator.GenerateTitle(productNameRU, Language.RU);
            ViewBag.KeywordsRU = _seoGenerator.GenerateKeywords(productNameRU, Language.RU);
            ViewBag.DescriptionRU = _seoGenerator.GenerateDescription(productNameRU, Language.RU);

            return View("Index");
        }
    }
}
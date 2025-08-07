using Application.Interfaces;
using Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    internal sealed class SeoGeneratorService : ISeoGeneratorService
    {
        private readonly ILogger<SeoGeneratorService> _logger;
        private readonly string _shopNameLatin = "ASSOL";
        private readonly string _shopNameCyrillic = "АССОЛЬ";
        private readonly Dictionary<Language, string> _headingTemplates = new()
        {
            { Language.UA, "{0} – Огляд, характеристики, питання та відгуки" },
            { Language.RU, "{0} – Обзор, характеристики, вопросы и отзывы" }
        };
        private readonly Dictionary<Language, string> _titleTemplates = new()
        {
            { Language.UA, "{0} – Купити в магазині {1} з доставкою по Україні" },
            { Language.RU, "{0} – Купить в магазине {1} с доставкой по Украине" }
        };
        private readonly Dictionary<Language, string> _keywordsTemplates = new()
        {
            { Language.UA, "купити {0}, {0} {1}, {0} ціна, {0} відгуки" },
            { Language.RU, "купить {0}, {0} {1}, {0} цена, {0} отзывы" }
        };
        private readonly Dictionary<Language, string> _descriptionTemplates = new()
        {
            { Language.UA, "{1} ► ✔️{0}✔️ ► Вигідна ціна ⭐️ Характеристика та обзор⭐️ У розстрочку або оплату частинами ✨ Доставка по Україні." },
            { Language.RU, "{1} ► ✔️{0}✔️ ► Выгодная цена ⭐️ Характеристика и обзор⭐️ В рассрочку или оплату по частям ✨ Доставка по Украине." }
        };

        public SeoGeneratorService(ILogger<SeoGeneratorService> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public string GenerateHeading(string productName, Language lang = Language.UA) =>
            ApplyTemplate(_headingTemplates[lang], productName.Trim(), _shopNameCyrillic);

        public string GenerateTitle(string productName, Language lang = Language.UA) =>
            ApplyTemplate(_titleTemplates[lang], productName.Trim(), _shopNameCyrillic);

        public string GenerateKeywords(string productName, Language lang = Language.UA) =>
            ApplyTemplate(_keywordsTemplates[lang], productName.Trim(), _shopNameCyrillic);

        public string GenerateDescription(string productName, Language lang = Language.UA) =>
            ApplyTemplate(_descriptionTemplates[lang], productName.Trim(), _shopNameCyrillic);

        private static string ApplyTemplate(string template, string productName, string shopName)
        {
            if (string.IsNullOrWhiteSpace(productName))
                return string.Empty;

            return string.Format(template, productName, shopName);
        }
    }
}
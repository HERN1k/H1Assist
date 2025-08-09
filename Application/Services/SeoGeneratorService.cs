using Application.Interfaces;
using Domain.ValueObjects;

namespace Application.Services
{
    internal sealed class SeoGeneratorService : ISeoGeneratorService
    {
        private readonly string _shopNameCyrillic = "Ассоль";
        private readonly Dictionary<Language, string> _headingTemplates = new()
        {
            { Language.UA, "{0}" },
            { Language.RU, "{0}" }
        };
        private readonly Dictionary<Language, string> _titleTemplates = new()
        {
            { Language.UA, "{0} купити в інтернет магазині Ассоль | Ціни, відгуки, характеристики. Тільки офіційна техніка" },
            { Language.RU, "{0} купить в интернет магазине Ассоль | Цены, отзывы, характеристики. Только официальная техника" }
        };
        private readonly Dictionary<Language, string> _keywordsTemplates = new()
        {
            { Language.UA, "купити {0}, {0} {1}, {0} ціна, {0} купити онлайн, {0} з доставкою, {0} відгуки" },
            { Language.RU, "купить {0}, {0} {1}, {0} цена, {0} купить онлайн, {0} с доставкой, {0} отзывы" }
        };
        private readonly Dictionary<Language, string> _descriptionTemplates = new()
        {
            { Language.UA, "{0} купити в інтернет магазині Ассоль✅ Розстрочка під 0%✅ Оплата частинами✅ Офіційна гарантія⚡ Швидка доставка в Київ, Одесу, Черкаси, Кропивницький та інші міста України. Магазин техніки для всієї родини 👉Ассоль" },
            { Language.RU, "{0} купить в интернет магазине Ассоль✅ Рассрочка под 0%✅ Оплата частями✅ Официальная гарантия⚡ Быстрая доставка в Киев, Одессу, Черкассы, Кропивницкий и в другие города Украины. Магазин техники для всей семьи 👉Ассоль" }
        };
        
        public SeoGeneratorService()
        {
            
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
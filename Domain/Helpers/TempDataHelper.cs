using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Domain.Helpers
{
    public static class TempDataHelper
    {
        public static T? Set<T>(this ITempDataDictionary tempData, string key, T? value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");

            tempData[key] = value;

            return value;
        }

        public static T Get<T>(this ITempDataDictionary tempData, string key, T fallback, Func<object?, T?>? func = null)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException(nameof(key), "Key cannot be null.");

            if (!tempData.TryGetValue(key, out object? value))
                return fallback;

            if (func != null)
                return func(value) ?? fallback;

            return value is T t ? t : fallback;
        }
    }
}
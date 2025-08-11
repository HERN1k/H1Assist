using Microsoft.Extensions.Caching.Memory;

namespace Application.Interfaces
{
    /// <summary>
    /// Represents a service for caching values in memory using <see cref="IMemoryCache"/>.<br/>
    /// Provides methods for getting, setting, and attempting to retrieve cached values.
    /// </summary>
    public interface ICacheService
    {
        /// <summary>
        /// Retrieves a value from the cache by the specified key.
        /// </summary>
        T? GetValue<T>(string key) where T : notnull;
        /// <summary>
        /// Attempts to retrieve a value from the cache by the specified key.
        /// </summary>
        bool TryGetValue<T>(string key, out T? value) where T : notnull;
        /// <summary>
        /// Caches a value with the specified key and optional expiration.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        T? SetValue<T>(string key, T value, TimeSpan expiration = default) where T : notnull;
        /// <summary>
        /// Removes the cached value associated with the specified key, if it exists.
        /// </summary>
        void RemoveValue(string key);
    }
}
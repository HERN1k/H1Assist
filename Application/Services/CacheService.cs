using Application.Interfaces;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    /// <summary>
    /// Represents a service for caching values in memory using <see cref="IMemoryCache"/>.<br/>
    /// Provides methods for getting, setting, and attempting to retrieve cached values.
    /// </summary>
    internal sealed class CacheService : ICacheService, IDisposable
    {
        private readonly IMemoryCache m_cache;
        private readonly ILogger<CacheService> m_logger;
        private readonly TimeSpan m_defaultExpiration;
        private bool m_disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CacheService"/> class.
        /// </summary>
        public CacheService(IMemoryCache cache, ILogger<CacheService> logger)
        {
            m_cache = cache ?? throw new ArgumentNullException(nameof(cache));
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_defaultExpiration = TimeSpan.FromMinutes(30.0D);
        }

        /// <summary>
        /// Retrieves a value from the cache by the specified key.
        /// </summary>
        public T? GetValue<T>(string key) where T : notnull
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                return default;
            }

            if (!m_cache.TryGetValue<T>(key, out T? value))
            {
                return default;
            }

            return value;
        }

        /// <summary>
        /// Attempts to retrieve a value from the cache by the specified key.
        /// </summary>
        public bool TryGetValue<T>(string key, out T? value) where T : notnull
        {
            value = GetValue<T>(key);

            return value != null;
        }

        /// <summary>
        /// Caches a value with the specified key and optional expiration.
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public T? SetValue<T>(string key, T value, TimeSpan expiration = default) where T : notnull
        {
            if (string.IsNullOrWhiteSpace(key))
            {
                throw new ArgumentNullException(nameof(key), "Cache key cannot be null");
            }

            if (value == null)
            {
                m_logger.LogWarning("Attempted to cache a null value for key \"{Key}\". Skipped.", key);

                return default;
            }

            if (expiration == default)
            {
                expiration = m_defaultExpiration;
            }

            m_cache.Set<T>(key, value, expiration);
            m_logger.LogDebug("Cached value of type {Type} with key \"{Key}\" for duration {Expiration}.", typeof(T).Name, key, expiration);

            return value;
        }

        /// <summary>
        /// Removes the cached value associated with the specified key, if it exists.
        /// </summary>
        public void RemoveValue(string key)
        {
            if (!string.IsNullOrWhiteSpace(key))
            {
                m_cache.Remove(key);
                m_logger.LogDebug("Removed cache entry with key \"{Key}\".", key);
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, 
        /// or resetting unmanaged resources.
        /// </summary>
        private void Dispose(bool disposing)
        {
            if (!m_disposedValue)
            {
                if (disposing)
                {
                    m_cache?.Dispose();
                }

                m_disposedValue = true;
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
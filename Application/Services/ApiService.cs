using System.Text.Json;
using Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    /// <summary>
    /// Handles sending HTTP requests with retry logic and deserialization of responses.
    /// </summary>
    internal class ApiService : IApiService
    {
        private readonly IHttpClientFactory m_httpFactory;
        private readonly ICacheService m_cache;
        private readonly ILogger<ApiService> m_logger;

        private const int m_maxRetries = 3;
        private const int m_delayMilliseconds = 500;

        /// <summary>
        /// Initializes a new instance of the <see cref="ApiService"/> class.
        /// </summary>
        public ApiService(IHttpClientFactory httpFactory, ICacheService cache, ILogger<ApiService> logger)
        {
            m_httpFactory = httpFactory ?? throw new ArgumentNullException(nameof(httpFactory));
            m_cache = cache ?? throw new ArgumentNullException(nameof(cache));
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Sends an HTTP GET request to a specified URL and attempts to deserialize the response.
        /// </summary>
        /// <param name="url">The URL to which the GET request will be sent.</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object of type T, or null if the request failed or the URL was invalid.</returns>
        public async Task<string?> GetAsync(string url, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return default;
            }

            if (m_cache.TryGetValue(url, out string? value))
            {
                return value;
            }

            string? result = await GetAsync("E-Katalog", new Uri(url), ct);

            if (result != null)
            {
                m_cache.SetValue(url, result);
            }
            else
            {
                m_logger.LogWarning("Failed to retrieve data from {Url}", url);
            }

            return result;
        }

        /// <summary>
        /// Sends an HTTP GET request to the specified URL and attempts to deserialize the response to the specified type.<br/>
        /// Includes retry logic and basic error handling.
        /// </summary>
        private async Task<string?> GetAsync(string httpClientName, Uri url, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(httpClientName) || url == null)
            {
                return default;
            }

            var http = m_httpFactory.CreateClient(httpClientName);

            for (int attempt = 1; attempt <= m_maxRetries; attempt++)
            {
                try
                {
                    ct.ThrowIfCancellationRequested();

                    HttpResponseMessage res = await http.GetAsync(url, ct);

                    if (!res.IsSuccessStatusCode)
                    {
                        m_logger.LogWarning("Received non-success status code {statusCode} from {url}", res.StatusCode, url);

                        if (attempt == m_maxRetries)
                        {
                            m_logger.LogError("Failed to get a successful response from {Url} after {Retries} attempts", url, m_maxRetries);

                            return default;
                        }

                        await Task.Delay(m_delayMilliseconds, ct);
                        continue;
                    }

                    ct.ThrowIfCancellationRequested();

                    string content = await res.Content.ReadAsStringAsync();

                    if (string.IsNullOrWhiteSpace(content))
                    {
                        m_logger.LogWarning("Empty response content from {Url}", url);

                        return default;
                    }

                    ct.ThrowIfCancellationRequested();

                    return content;
                }
                catch (TaskCanceledException) when (ct.IsCancellationRequested)
                {
                    return default;
                }
                catch (HttpRequestException ex)
                {
                    m_logger.LogWarning(ex, "{ex}", ex.Message);

                    if (attempt == m_maxRetries)
                    {
                        return default;
                    }

                    await Task.Delay(m_delayMilliseconds, ct);
                }
                catch (ArgumentNullException ex)
                {
                    m_logger.LogError(ex, "{ex}", ex.Message);

                    return default;
                }
                catch (JsonException ex)
                {
                    m_logger.LogError(ex, "{ex}", ex.Message);

                    return default;
                }
                catch (Exception ex)
                {
                    m_logger.LogError(ex, "{ex}", ex.Message);

                    return default;
                }
            }

            return default;
        }
    }
}
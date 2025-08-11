namespace Application.Interfaces
{
    /// <summary>
    /// Handles sending HTTP requests with retry logic and deserialization of responses.
    /// </summary>
    public interface IApiService
    {
        /// <summary>
        /// Sends an HTTP GET request to a specified URL and attempts to deserialize the response.
        /// </summary>
        /// <param name="url">The URL to which the GET request will be sent.</param>
        /// <param name="ct">A cancellation token to observe while waiting for the task to complete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the deserialized object of type T, or null if the request failed or the URL was invalid.</returns>
        Task<string?> GetAsync(string url, CancellationToken ct = default);
    }
}
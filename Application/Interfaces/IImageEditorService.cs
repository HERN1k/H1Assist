using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImageEditorService
    {
        Task<byte[]> ConvertImageFormatAsync(ImageExtension outputExtension, int selectedWidth, params IFormFile[] images);
        Task<byte[]> CreatePosterForVideoAsync(ImageExtension outputExtension, string url, string fileName);
        Task DownloadAndConvertImageFormatAsync(ImageExtension outputExtension, List<string> links, string outputDir);
        Task SaveBase64Image(string base64, string outputDir);
        Task<byte[]> CreateZipInMemoryAsync(string sourceDirectory);
    }
}
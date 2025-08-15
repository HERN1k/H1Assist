using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImageEditorService
    {
        Task<byte[]> ConvertImageFormatAsync(ImageExtension outputExtension, int selectedWidth, params IFormFile[] images);
        Task<byte[]> CreatePosterForVideoAsync(ImageExtension outputExtension, string url, string fileName);
    }
}
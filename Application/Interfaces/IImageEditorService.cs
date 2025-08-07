using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IImageEditorService
    {
        Task<byte[]> ConvertImageFormatAsync(ImageExtension outputExtension, params IFormFile[] images);
    }
}
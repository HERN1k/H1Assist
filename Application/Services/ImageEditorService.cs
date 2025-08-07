using System.Diagnostics;
using System.IO.Compression;
using Application.Interfaces;
using Domain.ValueObjects;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    internal sealed class ImageEditorService : IImageEditorService
    {
        private readonly ILogger<ImageEditorService> _logger;

        public ImageEditorService(ILogger<ImageEditorService> logger)
        {
            this._logger = logger ?? throw new ArgumentNullException(nameof(logger));   
        }

        public async Task<byte[]> ConvertImageFormatAsync(ImageExtension outputExtension, params IFormFile[] images)
        {
            if (images.Length <= 0)
            {
                throw new ArgumentException("At least one image must be provided.", nameof(images));
            }

            string sharedDir = "/app/shared";
            string id = Guid.NewGuid().ToString();
            string inputDir = Path.Combine(sharedDir, $"{id}_input");
            string outputDir = Path.Combine(sharedDir, $"{id}_output");
            
            try
            {
                Directory.CreateDirectory(inputDir);
                Directory.CreateDirectory(outputDir);

                foreach (var file in images)
                {
                    var filePath = Path.Combine(inputDir, file.FileName);
                    await using var stream = File.Create(filePath);

                    _logger.LogDebug("Copy the file with the name {fileName} to the folder {inputDir}.", file.FileName, inputDir);
                    await file.CopyToAsync(stream);
                }

                foreach (var file in images)
                {
                    await FfmpegConvertImageFormatAsync(
                        fileName: file.FileName,
                        inputDir: inputDir,
                        outputDir: outputDir,
                        outputExtension: outputExtension
                    );
                }

                return await CreateZipInMemoryAsync(outputDir);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while converting the image format.");
                throw new InvalidOperationException("An error occurred while converting the image format.", ex);
            }
            finally
            {
                if (Directory.Exists(inputDir))
                    Directory.Delete(inputDir, recursive: true);

                if (Directory.Exists(outputDir))
                    Directory.Delete(outputDir, recursive: true);
            }
        }

        private async Task FfmpegConvertImageFormatAsync(string fileName, string inputDir, string outputDir, ImageExtension outputExtension)
        {
            if (string.IsNullOrWhiteSpace(fileName) || !fileName.Contains('.'))
                throw new ArgumentException("Invalid file name.", nameof(fileName));

            if (!Directory.Exists(inputDir))
                throw new DirectoryNotFoundException($"Input directory does not exist: {inputDir}");

            if (!Directory.Exists(outputDir))
                throw new DirectoryNotFoundException($"Output directory does not exist: {outputDir}");

            if (!outputExtension.HasValue)
                throw new ArgumentException("Output extension must be specified.", nameof(outputExtension));

            string inputPath = Path.Combine(inputDir, fileName);
            string outputPath = Path.Combine(outputDir, Path.GetFileNameWithoutExtension(fileName)) + outputExtension;

            var psi = new ProcessStartInfo(
                fileName: "ffmpeg",
                arguments: $"-i \"{inputPath}\" \"{outputPath}\"")
            {
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = Process.Start(psi);

            if (process == null)
            {
                _logger.LogError("Failed to start ffmpeg process.");
                throw new InvalidOperationException("Failed to start ffmpeg process.");
            }

            string output = await process.StandardOutput.ReadToEndAsync();
            string error = await process.StandardError.ReadToEndAsync();

            _logger.LogDebug("Ffmpeg processes file: {fileName}", fileName);

            await process.WaitForExitAsync();

            if (process.ExitCode != 0)
            {
                _logger.LogError("Ffmpeg process failed: {Error}", error);
                throw new InvalidOperationException($"Ffmpeg process failed: {error}");
            }
        }

        private static async Task<byte[]> CreateZipInMemoryAsync(string sourceDirectory)
        {
            using var memoryStream = new MemoryStream();

            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
            {
                var files = Directory.GetFiles(sourceDirectory, "*", SearchOption.AllDirectories);

                foreach (var filePath in files)
                {
                    var relativePath = Path.GetRelativePath(sourceDirectory, filePath);

                    var entry = archive.CreateEntry(relativePath, CompressionLevel.Fastest);

                    await using var entryStream = entry.Open();
                    await using var fileStream = File.OpenRead(filePath);
                    await fileStream.CopyToAsync(entryStream);
                }
            }

            memoryStream.Seek(0, SeekOrigin.Begin);

            return memoryStream.ToArray();
        }
    }
}
using Microsoft.AspNetCore.Http;
using ShitChat.Shared.Enums;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Gif;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;
using System.Runtime.InteropServices;

namespace ShitChat.Application.Uploads.Services;

public class UploadService : IUploadService
{
    private readonly string _imageStoragePath = "/Uploads";

    public UploadService()
    {
        if (!Directory.Exists(_imageStoragePath))
        {
            Directory.CreateDirectory(_imageStoragePath);
        }
    }

    public async Task<(bool, UploadActionResult, string?)> UploadFileAsync(IFormFile file, int? width = null, int? height = null)
    {
        if (file == null || file.Length == 0)
            return (false, UploadActionResult.ErrorInvalidFile, null);

        string[] allowedExtensions = [".jpg", ".jpeg", ".png", ".webp", ".gif"];

        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
            return (false, UploadActionResult.ErrorInvalidFileFormat, null);

        string imageId = Guid.NewGuid().ToString();
        string imageName = fileExtension == ".gif" ? $"{imageId}.gif" : $"{imageId}.webp";
        string ImagePath = Path.Combine(_imageStoragePath, imageName);

        using var image = await Image.LoadAsync(file.OpenReadStream());

        if (fileExtension != ".gif" && width != null && height != null)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width.Value, height.Value),
                Mode = ResizeMode.Crop
            }));
        }

        using (var fileStream = new FileStream(ImagePath, FileMode.Create))

            if (fileExtension == ".gif")
            {
                await image.SaveAsGifAsync(fileStream, new GifEncoder());
            }
            else
            {

                await image.SaveAsWebpAsync(fileStream, new WebpEncoder
                {
                    FileFormat = WebpFileFormatType.Lossless
                });
            }


        return (true, UploadActionResult.SuccessFileUploaded, imageName);
    }
}

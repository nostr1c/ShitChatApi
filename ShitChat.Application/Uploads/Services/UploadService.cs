using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

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

    public async Task<(bool, string, string?)> UploadFileAsync(IFormFile file, int? width = null, int? height = null)
    {
        if (file == null || file.Length == 0)
            return (false, "ErrorInvalidFile", null);

        string[] allowedExtensions = [".jpg", ".jpeg", ".png"];

        var fileExtension = Path.GetExtension(file.FileName).ToLower();
        if (!allowedExtensions.Contains(fileExtension))
            return (false, "ErrorNotValidFileFormat", null);

        string imageId = Guid.NewGuid().ToString();
        string imageName = $"{imageId}{fileExtension}";
        string ImagePath = Path.Combine(_imageStoragePath, imageName);

        using var image = await Image.LoadAsync(file.OpenReadStream());

        if (width != null && height != null)
        {
            image.Mutate(x => x.Resize(new ResizeOptions
            {
                Size = new Size(width.Value, height.Value),
                Mode = ResizeMode.Crop
            }));
        }

        using (var fileStream = new FileStream(ImagePath, FileMode.Create))
        {
            await image.SaveAsWebpAsync(fileStream);
        }

        return (true, "SuccessFileUploaded", imageName);
    }
}

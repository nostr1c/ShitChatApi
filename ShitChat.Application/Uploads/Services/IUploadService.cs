using Microsoft.AspNetCore.Http;

namespace ShitChat.Application.Uploads.Services;

public interface IUploadService
{
    Task<(bool, string, string?)> UploadFileAsync(IFormFile file, int? width = null, int? height = null);
}

using Microsoft.AspNetCore.Http;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Uploads.Services;

public interface IUploadService
{
    Task<(bool, UploadActionResult, string?)> UploadFileAsync(IFormFile file, int? width = null, int? height = null);
}

using Microsoft.AspNetCore.Mvc;

namespace ShitChat.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class FilesController : ControllerBase
{
    private readonly string _imageStoragePath = "/Uploads";

    public FilesController(){}


    [HttpGet("{fileName}")]
    public IActionResult GetFile(string fileName)
    {
        var filePath = Path.Combine(_imageStoragePath, fileName);
        if (!System.IO.File.Exists(filePath))
            return NotFound("Image not found.");

        var fileExtension = Path.GetExtension(fileName).ToLower();

        string contentType = fileExtension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".webp" => "image/webp",
            ".gif" => "image/gif",
            _ => "application/octet-stream"
        };

        var imageStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
        return File(imageStream, contentType);
    }
}

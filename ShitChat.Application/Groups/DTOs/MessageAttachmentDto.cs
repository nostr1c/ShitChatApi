namespace ShitChat.Application.Groups.DTOs;

public class MessageAttachmentDto
{
    public required string FileName { get; set; }
    public required long FileSize { get; set; }
    public required string FileType { get; set; }
}

using System.ComponentModel.DataAnnotations.Schema;

namespace ShitChat.Domain.Entities;

public class MessageAttachment
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string FileName { get; set; }
    public string FileType { get; set; }
    public long FileSize { get; set; }
    public Guid MessageId { get; set; }

    // Navigation props
    [ForeignKey(nameof(MessageId))]
    public Message Message { get; set; }
}

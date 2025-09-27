using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace ShitChat.Application.Groups.Requests;

public class SendMessageRequest
{
    public string? Content { get; set; }
    public IFormFile? Attachment { get; set; }
}

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x)
            .Must(x => !string.IsNullOrWhiteSpace(x.Content) || x.Attachment != null)
            .WithMessage("ErrorMessageCannotBeEmpty");
    }
}
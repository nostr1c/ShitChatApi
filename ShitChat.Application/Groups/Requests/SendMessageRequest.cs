using FluentValidation;

namespace ShitChat.Application.Groups.Requests;

public class SendMessageRequest
{
    public required string Content { get; set; }
}

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("ErrorMessageCannotBeEmpty");
    }
}

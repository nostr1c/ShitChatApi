using FluentValidation;

namespace ShitChat.Application.Requests;

public class SendMessageRequest
{
    public string Content { get; set; }
}

public class SendMessageRequestValidator : AbstractValidator<SendMessageRequest>
{
    public SendMessageRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("ErrorMessageCannotBeEmpty");
    }
}

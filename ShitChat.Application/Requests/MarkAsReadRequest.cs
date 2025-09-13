using FluentValidation;

namespace ShitChat.Application.Requests;

public class MarkAsReadRequest
{
    public Guid LastMessageId { get; set; }
}

public class MarkAsReadRequestValidator : AbstractValidator<MarkAsReadRequest>
{
    public MarkAsReadRequestValidator()
    {
        RuleFor(x => x.LastMessageId).NotEmpty().WithMessage("ErrorLastMessageIdCannotBeEmpty");
    }
}   

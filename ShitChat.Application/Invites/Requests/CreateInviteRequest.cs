using FluentValidation;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Invites.Requests;

public class CreateInviteRequest
{
    public DateOnly ValidThrough { get; set; }
}

public class CreateInviteRequestValidator : AbstractValidator<CreateInviteRequest>
{
    public CreateInviteRequestValidator()
    {
        RuleFor(x => x.ValidThrough)
            .NotEmpty()
            .WithMessage(InviteActionResult.ErrorValidThroughCannotBeEmpty.ToString())
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage(InviteActionResult.ErrorValidThroughMustBeAFutureDate.ToString());
    }
}
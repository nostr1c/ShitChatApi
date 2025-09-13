using FluentValidation;

namespace ShitChat.Application.Requests;

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
            .WithMessage("ErrorValidThroughCannotBeEmpty")
            .Must(date => date >= DateOnly.FromDateTime(DateTime.UtcNow))
            .WithMessage("ErrorValidThroughMustBeAFutureDate");
    }
}
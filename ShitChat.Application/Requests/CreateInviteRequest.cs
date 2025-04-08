using FluentValidation;

namespace ShitChat.Application.Requests;

public class CreateInviteRequest
{
    public string ValidThrough { get; set; }
}

public class CreateInviteRequestValidator : AbstractValidator<CreateInviteRequest>
{
    public CreateInviteRequestValidator()
    {
        RuleFor(x => x.ValidThrough)
            .NotEmpty()
            .WithMessage("ErrorValidThroughCannotBeEmpty");
        RuleFor(x => x.ValidThrough);
    }
}
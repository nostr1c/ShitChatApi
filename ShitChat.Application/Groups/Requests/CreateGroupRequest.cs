using FluentValidation;

namespace ShitChat.Application.Groups.Requests;

public class CreateGroupRequest
{
    public required string Name { get; set; }
}

public class CreateGroupRequestValidator : AbstractValidator<CreateGroupRequest>
{
    public CreateGroupRequestValidator()
    {
        RuleFor(x => x.Name)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
                .WithMessage("ErrorGroupNameCannotBeEmpty")
            .MinimumLength(2)
                .WithMessage("ErrorGroupNameMinLength");
    }
}

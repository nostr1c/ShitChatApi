using FluentValidation;

namespace api.Models.Requests;

public class CreateGroupRequest
{
    public string Name { get; set; }
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

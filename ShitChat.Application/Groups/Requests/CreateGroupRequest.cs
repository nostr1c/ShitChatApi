using FluentValidation;
using ShitChat.Shared.Enums;

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
                .WithMessage(GroupActionResult.ErrorGroupNameCannotBeEmpty.ToString())
            .MinimumLength(2)
                .WithMessage(GroupActionResult.ErrorGroupNameMinLength.ToString());
    }
}

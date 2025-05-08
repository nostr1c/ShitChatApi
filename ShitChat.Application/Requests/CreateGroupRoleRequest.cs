using FluentValidation;

namespace ShitChat.Application.Requests;

public class CreateGroupRoleRequest
{
    public string Name { get; set; }
    public string? Color { get; set; }
}

public class CreateGroupRoleRequestValidator : AbstractValidator<CreateGroupRoleRequest>
{
    public CreateGroupRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("ErrorGroupRoleNameCannotBeEmpty");
    }
}

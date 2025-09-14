using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ShitChat.Application.Requests;

public class CreateGroupRoleRequest
{
    public string Name { get; set; }
    public string Color { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class CreateGroupRoleRequestValidator : AbstractValidator<CreateGroupRoleRequest>
{
    public CreateGroupRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("ErrorGroupRoleNameCannotBeEmpty");
        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("ErrorGroupRoleColorCannotBeEmpty");
    }
}

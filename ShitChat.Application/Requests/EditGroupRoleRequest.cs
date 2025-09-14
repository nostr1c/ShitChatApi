using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace ShitChat.Application.Requests;

public class EditGroupRoleRequest
{
    public string Name { get; set; }
    public string Color { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class EditGroupRoleRequestValidator : AbstractValidator<EditGroupRoleRequest>
{
    public EditGroupRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("ErrorGroupRoleNameCannotBeEmpty");
        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage("ErrorGroupRoleColorCannotBeEmpty");
    }
}

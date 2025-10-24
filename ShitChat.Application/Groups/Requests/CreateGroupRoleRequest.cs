using FluentValidation;
using ShitChat.Shared.Enums;
using System.ComponentModel.DataAnnotations;

namespace ShitChat.Application.Groups.Requests;

public class CreateGroupRoleRequest
{
    public required string Name { get; set; }
    public required string Color { get; set; }
    public List<string> Permissions { get; set; } = new();
}

public class CreateGroupRoleRequestValidator : AbstractValidator<CreateGroupRoleRequest>
{
    public CreateGroupRoleRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(GroupActionResult.ErrorGroupRoleNameCannotBeEmpty.ToString());
        RuleFor(x => x.Color)
            .NotEmpty()
            .WithMessage(GroupActionResult.ErrorGroupRoleColorCannotBeEmpty.ToString());
    }
}

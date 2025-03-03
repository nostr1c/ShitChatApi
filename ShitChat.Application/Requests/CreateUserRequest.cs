using Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace Application.Requests;

public class CreateUserRequest
{
    public string Username { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly UserManager<User> _userManager;

    public CreateUserRequestValidator(UserManager<User> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.Username)
            .NotEmpty()
                .WithMessage("ErrorUsernameCannotBeEmpty")
            .MustAsync(async (username, cancellation) =>
            {
                var user = await _userManager.FindByNameAsync(username);
                return user == null;
            }).WithMessage("ErrorUsernameAlreadyExists");

        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage("ErrorEmailCannotBeEmpty")
            .EmailAddress()
                .WithMessage("ErrorEmailNotValid")
            .MustAsync(async (email, cancellation) =>
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user == null;
            }).WithMessage("ErrorEmailAlreadyExists");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("ErrorPasswordCannotBeEmpty")
            .MinimumLength(6)
                .WithMessage("ErrorPasswordMinLength");
    }
}
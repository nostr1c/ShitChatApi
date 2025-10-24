using ShitChat.Domain.Entities;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Auth.Requests;

public class CreateUserRequest
{
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
}

public class CreateUserRequestValidator : AbstractValidator<CreateUserRequest>
{
    private readonly UserManager<User> _userManager;

    public CreateUserRequestValidator(UserManager<User> userManager)
    {
        _userManager = userManager;

        RuleFor(x => x.Username)
            .NotEmpty()
                .WithMessage(AuthActionResult.ErrorUsernameCannotBeEmpty.ToString())
            .MustAsync(async (username, cancellation) =>
            {
                var user = await _userManager.FindByNameAsync(username);
                return user == null;
            }).WithMessage(AuthActionResult.ErrorUsernameAlreadyExists.ToString());

        RuleFor(x => x.Email)
            .NotEmpty()
                .WithMessage(AuthActionResult.ErrorEmailCannotBeEmpty.ToString())
            .EmailAddress()
                .WithMessage(AuthActionResult.ErrorEmailNotValid.ToString())
            .MustAsync(async (email, cancellation) =>
            {
                var user = await _userManager.FindByEmailAsync(email);
                return user == null;
            }).WithMessage(AuthActionResult.ErrorEmailAlreadyExists.ToString());

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage(AuthActionResult.ErrorPasswordCannotBeEmpty.ToString())
            .MinimumLength(6)
                .WithMessage(AuthActionResult.ErrorPasswordMinLength.ToString());
    }
}
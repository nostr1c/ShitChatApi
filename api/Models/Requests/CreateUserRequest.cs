using api.Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace api.Models.Requests
{
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
                .MustAsync(async (username, cancellation) =>
                {
                    var user = await _userManager.FindByNameAsync(username);
                    return user == null;
                }).WithMessage("User with username already exists");

            RuleFor(x => x.Email)
                .EmailAddress()
                    .NotEmpty()
                    .WithMessage("Email is not a valid email")
                    .MustAsync(async (email, cancellation) =>
                    {
                        var user = await _userManager.FindByEmailAsync(email);
                        return user == null;
                    }).WithMessage("User with email already exists");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                    .WithMessage("Password must atleast be 6 characters.");
        }
    }
}
using api.Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace api.Models.Requests
{
    public class UpdateUserRequest
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Avatar { get; set; }
    }

    public class UpdateUserRequestValidator : AbstractValidator<UpdateUserRequest>
    {
        private readonly UserManager<User> _userManager;

        public UpdateUserRequestValidator(UserManager<User> userManager)
        {
            _userManager = userManager;

            RuleFor(x => x.Username)
                .NotEmpty()
                .MustAsync(async (username, cancellation) =>
                {
                    var user = await _userManager.FindByNameAsync(username);
                    return user == null;
                }).WithMessage("User with username already exists");
        }
    }
}

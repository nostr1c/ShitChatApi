using FluentValidation;

namespace api.Models.Requests
{
    public class LoginUserRequest
    {
        public string Email { get; set; }

        public string Password { get; set; }
    }

    public class RequestLoginValidator : AbstractValidator<LoginUserRequest>
    {
        public RequestLoginValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                    .WithMessage("Email address is not valid")
                .NotEmpty()
                    .WithMessage("Email cannot be empty");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                    .WithMessage("Password must atleast be 6 characters");
        }
    }
}

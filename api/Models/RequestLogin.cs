using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace api.Models
{
    public class RequestLogin
    {
        [Required]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }

    public class RequestLoginValidator : AbstractValidator<RequestLogin>
    {
        public RequestLoginValidator()
        {
            RuleFor(x => x.Email)
                .EmailAddress()
                    .NotEmpty()
                    .WithMessage("Email is not a valid email");

            RuleFor(x => x.Password)
                .MinimumLength(6)
                    .WithMessage("Password must atleast be 6 characters.");
        }
    }
}

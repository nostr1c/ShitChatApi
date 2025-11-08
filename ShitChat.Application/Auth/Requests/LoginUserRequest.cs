using FluentValidation;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Auth.Requests;

public class LoginUserRequest
{
    public required string EmailOrUsername { get; set; }

    public required string Password { get; set; }

}

public class RequestLoginValidator : AbstractValidator<LoginUserRequest>
{
    public RequestLoginValidator()
    {
        RuleFor(x => x.EmailOrUsername)
            .NotEmpty()
                .WithMessage(AuthActionResult.ErrorEmailCannotBeEmpty.ToString());

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage(AuthActionResult.ErrorPasswordCannotBeEmpty.ToString());
    }
}

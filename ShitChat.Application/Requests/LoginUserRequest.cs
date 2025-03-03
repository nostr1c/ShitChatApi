using FluentValidation;

namespace ShitChat.Application.Requests;

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
            .NotEmpty()
                .WithMessage("ErrorEmailCannotBeEmpty");

        RuleFor(x => x.Password)
            .NotEmpty()
                .WithMessage("ErrorPasswordCannotBeEmpty");
    }
}

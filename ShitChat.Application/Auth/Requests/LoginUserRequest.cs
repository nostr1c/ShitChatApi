﻿using FluentValidation;

namespace ShitChat.Application.Auth.Requests;

public class LoginUserRequest
{
    public required string Email { get; set; }

    public required string Password { get; set; }

    public string? Username { get; set; }
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

using FluentValidation;

namespace ShitChat.Application.Users.Requests;

public class UpdateAvatarRequest
{
    public required string AvatarUri { get; set; }
}

public class UpdateAvatarRequestValidator : AbstractValidator<UpdateAvatarRequest>
{
    public UpdateAvatarRequestValidator()
    {
        RuleFor(x => x.AvatarUri)
            .NotEmpty().WithMessage("ErrorAvatarCannotBeEmpty");
            //.Matches("^(https?:\\/\\/)([a-zA-Z0-9.-]+)(:[0-9]+)?(\\/[^\\s]*)*\\.(jpg|gif|png)$\r\n")
            //    .WithMessage("Not a valid image url.");
    }
}

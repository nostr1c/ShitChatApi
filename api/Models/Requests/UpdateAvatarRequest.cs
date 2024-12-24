using api.Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Identity;

namespace api.Models.Requests
{
    public class UpdateAvatarRequest
    {
        public string AvatarUri { get; set; }
    }

    public class UpdateAvatarRequestValidator : AbstractValidator<UpdateAvatarRequest>
    {
        public UpdateAvatarRequestValidator()
        {
            RuleFor(x => x.AvatarUri)
                .NotEmpty();
                //.Matches("^(https?:\\/\\/)([a-zA-Z0-9.-]+)(:[0-9]+)?(\\/[^\\s]*)*\\.(jpg|gif|png)$\r\n")
                //    .WithMessage("Not a valid image url.");
        }
    }
}

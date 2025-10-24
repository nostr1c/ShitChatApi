
using FluentValidation;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Groups.Requests;

public class BanUserRequest
{
    public string? Reason { get; set; }
}

public class BanUserRequestValidator : AbstractValidator<BanUserRequest>
{
    public BanUserRequestValidator()
    {
        RuleFor(x => x.Reason)
            .MaximumLength(500)
                .WithMessage(GroupActionResult.ErrorBanReasonMaxLength.ToString());
    }
}

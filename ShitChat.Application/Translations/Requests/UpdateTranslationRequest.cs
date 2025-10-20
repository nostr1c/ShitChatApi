
using FluentValidation;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Translations.Requests;

public class UpdateTranslationRequest
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class UpdateTranslationRequestValidator : AbstractValidator<UpdateTranslationRequest>
{
    public UpdateTranslationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(TranslationActionResult.ErrorTranslationNameCannotBeEmpty.ToString());

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage(TranslationActionResult.ErrorTranslationValueCannotBeEmpty.ToString());
    }
}

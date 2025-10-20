
using FluentValidation;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Translations.Requests;

public class CreateTranslationRequest
{
    public string Name { get; set; }
    public string Value { get; set; }
}

public class CreateTranslationRequestValidator : AbstractValidator<CreateTranslationRequest>
{
    public CreateTranslationRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage(TranslationActionResult.ErrorTranslationNameCannotBeEmpty.ToString());

        RuleFor(x => x.Value)
            .NotEmpty()
            .WithMessage(TranslationActionResult.ErrorTranslationValueCannotBeEmpty.ToString());
    }
}

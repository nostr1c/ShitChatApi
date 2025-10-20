using ShitChat.Application.Translations.Dtos;
using ShitChat.Application.Translations.Requests;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Translations.Services;

public interface ITranslationService
{
    Task<(TranslationActionResult, IEnumerable<TranslationDto>)> GetTranslationsAsync();
    Task<(bool, TranslationActionResult, TranslationDto?)> CreateTranslationAsync(CreateTranslationRequest request);
    Task<(bool, TranslationActionResult, TranslationDto?)> GetTranslationByGuidAsync(Guid translationId);
    Task<(bool, TranslationActionResult, TranslationDto?)> UpdateTranslationAsync(Guid translationId, UpdateTranslationRequest request);
    Task<(bool, TranslationActionResult)> DeleteTranslationAsync(Guid translationId);
}

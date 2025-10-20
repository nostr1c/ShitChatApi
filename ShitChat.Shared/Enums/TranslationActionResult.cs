namespace ShitChat.Shared.Enums;

public enum TranslationActionResult
{
    SuccessGotTranslations,
    SuccessCreatedTranslation,
    SuccessDeletedTranslation,
    SuccessGotTranslation,
    SuccessUpdatedTranslation,
    ErrorTranslationNameCannotBeEmpty,
    ErrorTranslationValueCannotBeEmpty,
    ErrorTranslationNameAlreadyExists,
    ErrorTranslationNotFound
}

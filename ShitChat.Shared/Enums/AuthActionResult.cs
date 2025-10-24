namespace ShitChat.Shared.Enums;

public enum AuthActionResult
{
    SuccessLoggedIn,
    SuccessRefreshedToken,
    SuccessCreatingUser,
    SuccessLoggedOut,
    SuccessGotCurrentUser,
    ErrorCreatingUser,
    ErrorInvalidEmailOrPassword,
    ErrorRefreshTokenNull,
    ErrorInvalidRefreshTokenFormat,
    ErrorInvalidRefreshTokenId,
    ErrorInvalidOrExpiredRefreshToken,
    ErrorRefreshTokenMissing,
    ErrorLoggedInUser,
    ErrorUsernameCannotBeEmpty,
    ErrorUsernameAlreadyExists,
    ErrorEmailCannotBeEmpty,
    ErrorEmailNotValid,
    ErrorEmailAlreadyExists,
    ErrorPasswordCannotBeEmpty,
    ErrorPasswordMinLength
}

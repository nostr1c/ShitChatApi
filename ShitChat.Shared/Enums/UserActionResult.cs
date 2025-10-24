namespace ShitChat.Shared.Enums;

public enum UserActionResult
{
    ErrorUserNotFound,
    SuccessGotUser,
    SuccessUpdatedAvatar,
    SuccessGotUserConnections,
    SuccessGotUserGroups,
    ErrorAvatarCannotBeEmpty
}

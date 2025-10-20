namespace ShitChat.Shared.Enums;

public enum ConnectionActionResult
{
    SuccessCreatingConnection,
    SuccessAcceptingConnection,
    ErrorFriendNotFound,
    ErrorCantAddYouself,
    ErrorConnectionAlreadyExists,
    ErrorFriendRequestNotFound,
    ErrorFriendRequestAlreadyAccepted,
    SuccessRemovingConnection
}

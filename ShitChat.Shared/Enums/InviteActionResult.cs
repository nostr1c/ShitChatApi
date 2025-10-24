namespace ShitChat.Shared.Enums;
public enum InviteActionResult
{
    SuccessCreatedInvite,
    SuccessDeletedInvite,
    SuccessGotGroupInvites,
    ErrorGroupNotFound,
    ErrorInviteNotFound,
    ErrorValidThroughCannotBeEmpty,
    ErrorValidThroughMustBeAFutureDate
}

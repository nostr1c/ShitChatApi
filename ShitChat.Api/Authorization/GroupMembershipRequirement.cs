using Microsoft.AspNetCore.Authorization;

namespace ShitChat.Api.Authorization;

public class GroupMembershipRequirement : IAuthorizationRequirement
{
    public string GroupIdParameter { get; }

    public GroupMembershipRequirement(string groupIdParameter = "groupGuid")
    {
        GroupIdParameter = groupIdParameter;
    }
}

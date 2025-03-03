using Microsoft.AspNetCore.Authorization;

namespace Api.Authorization;

public class GroupMembershipRequirement : IAuthorizationRequirement
{
    public string GroupIdParameter { get; }

    public GroupMembershipRequirement(string groupIdParameter = "groupGuid")
    {
        GroupIdParameter = groupIdParameter;
    }
}

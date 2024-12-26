using Microsoft.AspNetCore.Authorization;

namespace api.Extensions.Authorization
{
    public class GroupMembershipRequirement : IAuthorizationRequirement
    {
        public string GroupIdParameter { get; }

        public GroupMembershipRequirement(string groupIdParameter = "groupGuid")
        {
            GroupIdParameter = groupIdParameter;
        }
    }
}

using api.Models.Dtos;
using api.Models.Requests;

namespace api.Services.Interfaces
{
    public interface IGroupService
    {
        Task<GroupDto> CreateGroupAsync(CreateGroupRequest request);
    }
}

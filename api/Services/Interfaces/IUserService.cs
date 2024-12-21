using api.Data.Models;
using api.Models.Requests;

namespace api.Services.Interfaces
{
    public interface IUserService
    {
        public Task<(bool, User?)> GetUserByGuidAsync(string userGuid);
        public Task<(bool, User?)> UpdateUserByGuidAsync(UpdateUserRequest request);
    }
}

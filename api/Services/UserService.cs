using api.Data;
using api.Data.Models;
using api.Models.Requests;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;

        public UserService(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<(bool, User?)> GetUserByGuidAsync(string userGuid)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == userGuid);

            if (user == null)
                return (false, null);

            return (true, user);
        }

        public async Task<(bool, User?)> UpdateUserByGuidAsync(UpdateUserRequest request)
        {
            var user = await _dbContext.Users.SingleOrDefaultAsync(x => x.Id == request.Id);
            if (user == null)
                return (false, null);

            user.UserName = request.Username;
            user.AvatarUri = request.Avatar;

            _dbContext.Users.Update(user);

            return (true, user);
        }
    }
}

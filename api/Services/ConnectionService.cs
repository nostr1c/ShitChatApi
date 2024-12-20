using api.Data.Models;
using api.Data;
using Microsoft.AspNetCore.Identity;
using api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace api.Services
{
    public class ConnectionService : IConnectionService
    {
        private UserManager<User> _userManager;
        private AppDbContext _appDbContext;

        public ConnectionService
        (
            UserManager<User> userManager,
            AppDbContext appDbContext

        )
        {
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        public async Task<(bool Success, string Message)> CreateConnectionAsync(string userName, string friendId)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
                return (false, "Logged-in user not found.");

            var friend = await _userManager.FindByIdAsync(friendId);
            if (friend == null)
                return (false, "Friend could not be found.");

            if (user.Id == friend.Id)
                return (false, "You can't add yourself as a friend.");

            var connectionExists = await _appDbContext.Connections
                .AnyAsync(c => c.UserId == user.Id && c.FriendId == friend.Id);

            if (connectionExists)
                return (false, "This connection already exists");

            Connection connection = new Connection
            {
                UserId = user.Id,
                FriendId = friendId
            };

            await _appDbContext.Connections.AddAsync(connection);
            await _appDbContext.SaveChangesAsync();

            return (true, "Connection successfully added.");
        }
    }
}

using api.Repositories;
using api.Models;

namespace api.Services
{
    public class UserService
    {
        private readonly UserRepository userRepository;

        public UserService(UserRepository userController)
        {
            userRepository = userController;
        }

        public async Task<IEnumerable<User>> GetUsersAsync()
        {
            var users = await userRepository.GetUsersAsync();
            return users;
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var user = await userRepository.GetUserByIdAsync(userId);
            return user;
        }
    }
}

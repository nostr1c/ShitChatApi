using api.Repositories;
using api.Models;
using api.Dtos;

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

        public async Task<User> CreateUserAsync(CreateUserRequest createUserRequest)
        {
            var user = await userRepository.CreateUserAsync(createUserRequest);
            return user;
        }

        public async Task<User> UpdateUserAsync(UpdateUserRequest updateUserRequest)
        {
            var user = await userRepository.UpdateUserAsync(updateUserRequest);
            return user;
        }
    }
}

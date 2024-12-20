using api.Data.Models;
using api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public AccountService
        (
            UserManager<User> userManager,
            SignInManager<User> signInManager
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public ICollection<User> GetAll()
        {
            var users = _userManager.Users.ToList();
            return users;
        }

        public async Task<User?> RegisterUserAsync(CreateUserRequest request)
        {
            var user = new User
            {
                UserName = request.Username,
                Email = request.Email
            };

            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded)
            {
                return null;
            }
            // Check errors

            return user;
        }
    }
}

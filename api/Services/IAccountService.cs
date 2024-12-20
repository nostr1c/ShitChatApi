using api.Data.Models;
using api.Models;
using Microsoft.AspNetCore.Mvc;

namespace api.Services
{
    public interface IAccountService
    {
        ICollection<User> GetAll();
        Task<User?> RegisterUserAsync(CreateUserRequest request);
    }
}

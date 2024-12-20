using api.Data.Models;
using api.Models.Dtos;
using api.Models.Requests;

namespace api.Services.Interfaces
{
    public interface IAuthService
    {
        Task<User?> RegisterUserAsync(CreateUserRequest request);

        Task<(bool, string, LoginUserDto)> LoginUserAsync(LoginUserRequest request);
    }
}

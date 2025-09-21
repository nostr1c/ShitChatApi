using ShitChat.Domain.Entities;
using Microsoft.AspNetCore.Http;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Auth.DTOs;
using ShitChat.Application.Auth.Requests;

namespace ShitChat.Application.Auth.Services;

public interface IAuthService
{
    Task<User?> RegisterUserAsync(CreateUserRequest request);

    Task<(bool, string, LoginUserDto?)> LoginUserAsync(LoginUserRequest request);

    Task<TokenDto> CreateToken(User user);

    Task<(bool, string, TokenDto?)> RefreshToken(TokenDto tokenDto);

    void SetTokensInsideCookie(TokenDto tokenDto, HttpContext context);

    Task<UserDto?> GetCurrentUserAsync();
}

using ShitChat.Domain.Entities;
using Microsoft.AspNetCore.Http;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Auth.DTOs;
using ShitChat.Application.Auth.Requests;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Auth.Services;

public interface IAuthService
{
    Task<(bool, AuthActionResult, CreateUserDto?)> RegisterUserAsync(CreateUserRequest request);

    Task<(bool, AuthActionResult, LoginUserDto?)> LoginUserAsync(LoginUserRequest request);

    Task<TokenDto> CreateToken(User user);

    Task<(bool, AuthActionResult, TokenDto?)> RefreshToken(TokenDto tokenDto);

    void SetTokensInsideCookie(TokenDto tokenDto, HttpContext context);

    Task<UserDto?> GetCurrentUserAsync();
}

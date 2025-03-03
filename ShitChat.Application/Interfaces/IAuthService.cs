﻿using ShitChat.Application.DTOs;
using ShitChat.Domain.Entities;
using ShitChat.Application.Requests;
using Microsoft.AspNetCore.Http;

namespace ShitChat.Application.Interfaces;

public interface IAuthService
{
    Task<User?> RegisterUserAsync(CreateUserRequest request);

    Task<(bool, string, LoginUserDto)> LoginUserAsync(LoginUserRequest request);

    Task<TokenDto> CreateToken(string userId, bool populateExpiry);

    Task<(bool, TokenDto?)> RefreshToken(TokenDto tokenDto);

    void SetTokensInsideCookie(TokenDto tokenDto, HttpContext context);

    Task<UserDto?> GetCurrentUserAsync();
}

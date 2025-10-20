using ShitChat.Infrastructure.Data;
using ShitChat.Domain.Entities;
using ShitChat.Shared.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Auth.DTOs;
using ShitChat.Application.Auth.Requests;
using ShitChat.Shared.Enums;

namespace ShitChat.Application.Auth.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _config;
    private readonly AppDbContext _dbContext;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<AuthService> _logger;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService
    (
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration config,
        AppDbContext dbContext,
        IHttpContextAccessor httpContextAccessor,
        ILogger<AuthService> logger,
        IPasswordHasher<User> passwordHasher
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
        _passwordHasher = passwordHasher;
    }
    public async Task<(bool, AuthActionResult, CreateUserDto?)> RegisterUserAsync(CreateUserRequest request)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return (false, AuthActionResult.ErrorCreatingUser, null);

        var userDto = new CreateUserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
        };

        return (true, AuthActionResult.SuccessCreatingUser, userDto);
    }

    public async Task<(bool, AuthActionResult, LoginUserDto?)> LoginUserAsync(LoginUserRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return (false, AuthActionResult.ErrorInvalidEmailOrPassword, null);

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
            return (false, AuthActionResult.ErrorInvalidEmailOrPassword, null);

        var token = await CreateToken(user);

        var userDto = new LoginUserDto
        {
            Id = user.Id,
            Token = token,
        };

        return (true, AuthActionResult.SuccessLoggedIn, userDto);
    }
    public async Task<TokenDto> CreateToken(User user)
    {
        var jwtKey = _config["Jwt:Key"];
        var jwtIssuer = _config["Jwt:Issuer"];

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey!);
        var signingKey = new SymmetricSecurityKey(keyBytes);

        var token = new JwtSecurityToken(
            issuer: jwtIssuer,
            audience: jwtIssuer,
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256)
        );

        var refreshTokenRaw = CreateRefreshToken();
        var refreshTokenHash = _passwordHasher.HashPassword(user, refreshTokenRaw);

        var refreshToken = new RefreshToken
        {
            UserId = user.Id,
            TokenHash = refreshTokenHash,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            CreatedAt = DateTime.UtcNow,
        };

        _dbContext.RefreshTokens.Add(refreshToken);
        await _dbContext.SaveChangesAsync();

        return new TokenDto
        {
            AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
            RefreshToken = $"{refreshToken.Id}:{refreshTokenRaw}"
        };
    }

    public async Task<(bool, AuthActionResult, TokenDto?)> RefreshToken(TokenDto tokenDto)
    {
        if (string.IsNullOrEmpty(tokenDto.RefreshToken))
            return (false, AuthActionResult.ErrorRefreshTokenNull, null);

        var parts = tokenDto.RefreshToken.Split(":");
        if (parts.Length != 2)
            return (false, AuthActionResult.ErrorInvalidRefreshTokenFormat, null);

        if (!Guid.TryParse(parts[0], out var tokenId))
            return (false, AuthActionResult.ErrorInvalidRefreshTokenId, null);

        var secret = parts[1];

        var dbToken = await _dbContext.RefreshTokens
            .Include(r => r.User)
            .FirstOrDefaultAsync(rt => rt.Id == tokenId && rt.ExpiresAt > DateTime.UtcNow);

        if (dbToken == null)
                return (false, AuthActionResult.ErrorInvalidOrExpiredRefreshToken, null);

        var result = _passwordHasher.VerifyHashedPassword(dbToken.User, dbToken.TokenHash, secret);
        if (result != PasswordVerificationResult.Success)
            return (false, AuthActionResult.ErrorInvalidOrExpiredRefreshToken, null);

        _dbContext.RefreshTokens.Remove(dbToken);
        await _dbContext.SaveChangesAsync();

        var newTokenDto = await CreateToken(dbToken.User);
        return (true, AuthActionResult.SuccessRefreshedToken, newTokenDto);
    }

    public void SetTokensInsideCookie(TokenDto tokenDto, HttpContext context)
    {
        context.Response.Cookies.Append("accessToken", tokenDto.AccessToken!,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

        context.Response.Cookies.Append("refreshToken", tokenDto.RefreshToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });
    }

    public async Task<UserDto?> GetCurrentUserAsync()
    {
        var userId = _httpContextAccessor.GetUserId();
        var user = await _dbContext.Users.AsNoTracking().SingleOrDefaultAsync(x => x.Id == userId);
        if (user is null)
            return null;

        var userDto = new UserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email,
            Avatar = user.AvatarUri,
            CreatedAt = user.CreatedAt
        };

        return userDto;
    }

    private string CreateRefreshToken()
    {
        var randomNumber = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);

            return Convert.ToBase64String(randomNumber);
        }
    }
}
using ShitChat.Infrastructure.Data;
using ShitChat.Domain.Entities;
using ShitChat.Shared.Extensions;
using ShitChat.Application.DTOs;
using ShitChat.Application.Requests;
using ShitChat.Application.Interfaces;
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

namespace ShitChat.Application.Services;

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

    public async Task<User?> RegisterUserAsync(CreateUserRequest request)
    {
        var user = new User
        {
            UserName = request.Username,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (!result.Succeeded)
            return null;

        // Check errors

        return user;
    }

    public async Task<(bool, string, LoginUserDto)> LoginUserAsync(LoginUserRequest request)
    {
        bool success;
        string message = "";

        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null)
        {
            success = false;
            message = "Invalid Email or Password";
            return (success, message, null);
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded)
        {
            success = false;
            message = "Invalid Email or Password";
            return (success, message, null);
        }

        var token = await CreateToken(user);

        var userDto = new LoginUserDto
        {
            Id = user.Id,
            Token = token,
        };

        success = true;
        message = "Successfully logged in.";

        return (success, message, userDto);
    }
    public async Task<TokenDto> CreateToken(User user)
    {
        var jwtKey = _config["Jwt:Key"];
        var jwtIssuer = _config["Jwt:Issuer"];

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var keyBytes = Encoding.UTF8.GetBytes(jwtKey);
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

        var tokenDto = new TokenDto(new JwtSecurityTokenHandler().WriteToken(token), refreshTokenRaw);

        return tokenDto;
    }



    public async Task<(bool, string, TokenDto?)> RefreshToken(TokenDto tokenDto)
    {
        if (string.IsNullOrEmpty(tokenDto.RefreshTokenn))
            return (false, "ErrorRefreshTokenNull", null);

        var tokens = await _dbContext.RefreshTokens
            .AsNoTracking()
            .Include(r => r.User)
            .AsNoTracking()
            .Where(rt => rt.ExpiresAt > DateTime.UtcNow)
            .ToListAsync();

        var matchingToken = tokens.FirstOrDefault(rt => 
            _passwordHasher.VerifyHashedPassword(rt.User, rt.TokenHash, tokenDto.RefreshTokenn)
                == PasswordVerificationResult.Success);

        if (matchingToken == null)
            return (false, "ErrorInvalidOrExpiredRefreshToken", null);

        _dbContext.RefreshTokens.Remove(matchingToken);
        await _dbContext.SaveChangesAsync();

        var newTokenDto = await CreateToken(matchingToken.User);
        return (true, "SuccessRefreshedToken", newTokenDto);
    }

    public void SetTokensInsideCookie(TokenDto tokenDto, HttpContext context)
    {
        context.Response.Cookies.Append("accessToken", tokenDto.AccessToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddHours(1),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None
            });

        context.Response.Cookies.Append("refreshToken", tokenDto.RefreshTokenn,
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
        var user = await _userManager.FindByIdAsync(_httpContextAccessor.HttpContext.User.GetUserGuid());
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
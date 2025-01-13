using api.Data.Models;
using api.Helpers;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace api.Services;

public class AuthService : IAuthService
{
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IConfiguration _config;

    public AuthService
    (
        UserManager<User> userManager,
        SignInManager<User> signInManager,
        IConfiguration config
    )
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _config = config;
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

    public async Task<(bool, string, LoginUserDto)> LoginUserAsync(LoginUserRequest request)
    {
        bool success;
        string message = "";
        LoginUserDto userDto;

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

        var jwtKey = _config["Jwt:Key"];
        var jwtIssuer = _config["Jwt:Issuer"];
        var token = JwtHelper.GenerateJwtToken(user.Id, user.UserName, jwtKey, jwtIssuer);

        userDto = new LoginUserDto
        {
            Id = user.Id,
            Token = token,
        };

        success = true;
        message = "Successfully logged in.";

        return (success, message, userDto);
    }
}

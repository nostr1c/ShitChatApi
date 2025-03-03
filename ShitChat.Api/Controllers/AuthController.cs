using ShitChat.Application.DTOs;
using ShitChat.Application.Requests;
using ShitChat.Application.Interfaces;
using ShitChat.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;

namespace ShitChat.ShitChat.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IAuthService _authService;
    private readonly IServiceProvider _serviceProvider;

    public AuthController
    (
        IConfiguration config,
        IAuthService authService,
        IServiceProvider serviceProvider
    )
    {
        _config = config;
        _authService = authService;
        _serviceProvider = serviceProvider;
    }
    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("Register")]
    public async Task<ActionResult<GenericResponse<CreateUserDto>>> Register([FromBody] CreateUserRequest request)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<CreateUserRequest>>();
        var validationResult = await validator.ValidateAsync(request);
        var response = new GenericResponse<CreateUserDto>();

        if (!validationResult.IsValid)
        {
            response.Errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.ErrorMessage).ToList()
                );

            response.Message = "ErrorValidationFailed";
            return BadRequest(response);
        }

        var user = await _authService.RegisterUserAsync(request);
        if (user == null)
        {
            return BadRequest("ErrorCreatingUser");
        }

        CreateUserDto dto = new CreateUserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email
        };

        response.Data = dto;
        response.Message = "SuccessCreatingUser";

        return Ok(response);
    }

    /// <summary>
    /// Login a user
    /// </summary>
    [HttpPost("Login")]
    public async Task<ActionResult<GenericResponse<bool>>> Login([FromBody] LoginUserRequest request)
    {
        var validator = _serviceProvider.GetRequiredService<IValidator<LoginUserRequest>>();
        var validationResult = await validator.ValidateAsync(request);
        var response = new GenericResponse<bool>();

        if (!validationResult.IsValid)
        {
            response.Errors = validationResult.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Select(y => y.ErrorMessage).ToList()
                );

            response.Message = "ErrorValidationFailed";
            return BadRequest(response);
        }

        var (success, message, userDto) = await _authService.LoginUserAsync(request);

        if (!success)
        {
            response.Errors.Add("ErrorAuthenticatingUser", new List<string> { message });
            return Unauthorized(response);
        }

        _authService.SetTokensInsideCookie(userDto.Token, HttpContext);

        //response.Data = userDto;
        response.Data = true;
        response.Message = "SuccessLoggedIn";

        return Ok(response);
    }

    /// <summary>
    /// Logout a user
    /// </summary>
    [HttpPost("Logout")]
    public ActionResult<GenericResponse<string>> Logout()
    {
        Response.Cookies.Delete("accessToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        Response.Cookies.Delete("refreshToken", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None
        });

        var response = new GenericResponse<string>
        {
            Message = "Logged out successfully"
        };

        return Ok(response);
    }

    /// <summary>
    /// Refresh user token
    /// </summary>
    [HttpPost("Refresh")]
    public async Task<ActionResult<GenericResponse<string>>> Refresh()
    {
        var response = new GenericResponse<string>();

        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            response.Errors.Add("TokenError", new List<string> { "Refresh token is missing" });
            return Ok(response);

        }

        var tokenDto = new TokenDto(null, refreshToken);

        var (success, tokenDtoToReturn) = await _authService.RefreshToken(tokenDto);

        if (!success || tokenDtoToReturn is null)
        {
            response.Errors.Add("ErrorRefreshingSignIn", new List<string> { "Token bad request" });
            return Ok(response);
        }

        _authService.SetTokensInsideCookie(tokenDtoToReturn, HttpContext);

        response.Message = "Refreshed token";

        return Ok(response);
    }

    /// <summary>
    /// Get currently logged in  user
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("Me")]
    public async Task<ActionResult<GenericResponse<UserDto>>> GetCurrentUser()
    {
        var response = new GenericResponse<UserDto>();

        if (HttpContext.User.GetUserGuid() is null)
        {
            return Unauthorized();
        }

        var userDto = await _authService.GetCurrentUserAsync();
        if (userDto is null)
            return NotFound();

        response.Data = userDto;

        return Ok(response);
    }
}
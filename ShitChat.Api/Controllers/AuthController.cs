using ShitChat.Application.DTOs;
using ShitChat.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Auth.DTOs;
using ShitChat.Application.Auth.Requests;
using ShitChat.Application.Auth.Services;

namespace ShitChat.ShitChat.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController
    (
        IAuthService authService
    )
    {
        _authService = authService;
    }

    /// <summary>
    /// Register a new user
    /// </summary>
    [HttpPost("Register")]
    public async Task<ActionResult<GenericResponse<CreateUserDto>>> Register([FromBody] CreateUserRequest request)
    {
        var user = await _authService.RegisterUserAsync(request);

        if (user == null)
            return BadRequest(ResponseHelper.Error<CreateUserDto>("ErrorCreatingUser"));

        var loginRequest = new LoginUserRequest
        {
            Password = request.Password,
            Email = request.Email,
        };

        var (success, message, userDto) = await _authService.LoginUserAsync(loginRequest);

        if (!success || userDto is null)
            return BadRequest(ResponseHelper.Error<CreateUserDto>(message));

        _authService.SetTokensInsideCookie(userDto.Token, HttpContext);

        var createUserDto = new CreateUserDto
        {
            Id = user.Id,
            Username = user.UserName,
            Email = user.Email
        };

        return Ok(new GenericResponse<CreateUserDto>
        {
            Data = createUserDto,
            Message = "SuccessCreatingUser",
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Login a user
    /// </summary>
    [HttpPost("Login")]
    public async Task<ActionResult<GenericResponse<LoginUserDto?>>> Login([FromBody] LoginUserRequest request)
    {
        var (success, message, userDto) = await _authService.LoginUserAsync(request);

        if (!success || userDto == null)
            return BadRequest(ResponseHelper.Error<LoginUserDto?>(message));

        _authService.SetTokensInsideCookie(userDto.Token, HttpContext);

        return Ok(new GenericResponse<LoginUserDto>
        {
            Data = userDto,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Logout a user
    /// </summary>
    [HttpPost("Logout")]
    public ActionResult<GenericResponse<object?>> Logout()
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


        return Ok(new GenericResponse<object?>
        {
            Data = null,
            Message = "SuccessLoggedOut",
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Refresh user token
    /// </summary>
    [HttpPost("Refresh")]
    public async Task<ActionResult<GenericResponse<object?>>> Refresh()
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshToken))
        {
            Response.Headers["X-Auth-Status"] = "SessionExpired";
            return Unauthorized(ResponseHelper.Error<string>("ErrorRefreshTokenMissing"));
        }

        var (success, message, tokenDtoToReturn) = await _authService.RefreshToken(new TokenDto { AccessToken = null, RefreshToken = refreshToken });

        if (!success || tokenDtoToReturn is null)
        {
            Response.Headers["X-Auth-Status"] = "SessionExpired";
            return Unauthorized(ResponseHelper.Error<object?>(message));
        }

        _authService.SetTokensInsideCookie(tokenDtoToReturn, HttpContext);

        return Ok(new GenericResponse<object?>
        {
            Data = null,
            Message = message,
            Status = StatusCodes.Status200OK
        });
    }

    /// <summary>
    /// Get currently logged in  user
    /// </summary>
    [Authorize(AuthenticationSchemes = "Bearer")]
    [HttpGet("Me")]
    public async Task<ActionResult<GenericResponse<UserDto>>> GetCurrentUser()
    {
        if (HttpContext.User.GetUserGuid() is null)
        {
            Response.Headers["X-Auth-Status"] = "SessionExpired";
            return Unauthorized(ResponseHelper.Error<UserDto?>("ErrorLoggedInUser"));
        }

        var userDto = await _authService.GetCurrentUserAsync();
        if (userDto is null)
            return BadRequest(ResponseHelper.Error<UserDto?>("ErrorLoggedInUser"));

        return Ok(new GenericResponse<UserDto>
        {
            Data = userDto,
            Message = "SuccessGotCurrentUser",
            Status = StatusCodes.Status200OK
        });
    }
}
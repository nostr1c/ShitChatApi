using ShitChat.Application.DTOs;
using ShitChat.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ShitChat.Application.Users.DTOs;
using ShitChat.Application.Auth.DTOs;
using ShitChat.Application.Auth.Requests;
using ShitChat.Application.Auth.Services;
using ShitChat.Shared.Enums;

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
        var (success, message, userDto) = await _authService.RegisterUserAsync(request);

        if (success == false || userDto == null)
            return BadRequest(ResponseHelper.Error<CreateUserDto>(message));

        var loginRequest = new LoginUserRequest
        {
            Password = request.Password,
            Email = request.Email,
        };

        var (loginSuccess, loginMessage, loginUserDto) = await _authService.LoginUserAsync(loginRequest);

        if (loginSuccess == false || loginUserDto is null)
            return BadRequest(ResponseHelper.Error<CreateUserDto>(loginMessage));

        _authService.SetTokensInsideCookie(loginUserDto.Token, HttpContext);

        return Ok(ResponseHelper.Success(message, userDto, StatusCodes.Status201Created));
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

        return Ok(ResponseHelper.Success(message, userDto));
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


        return Ok(ResponseHelper.Success(AuthActionResult.SuccessLoggedOut));
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
            return Unauthorized(ResponseHelper.Error<string>(AuthActionResult.ErrorRefreshTokenMissing));
        }

        var (success, message, tokenDtoToReturn) = await _authService.RefreshToken(new TokenDto { AccessToken = null, RefreshToken = refreshToken });

        if (!success || tokenDtoToReturn is null)
        {
            Response.Headers["X-Auth-Status"] = "SessionExpired";
            return Unauthorized(ResponseHelper.Error<object?>(message));
        }

        _authService.SetTokensInsideCookie(tokenDtoToReturn, HttpContext);

        return Ok(ResponseHelper.Success(message, tokenDtoToReturn));
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
            return Unauthorized(ResponseHelper.Error<UserDto?>(AuthActionResult.ErrorLoggedInUser));
        }

        var userDto = await _authService.GetCurrentUserAsync();
        if (userDto is null)
            return BadRequest(ResponseHelper.Error<UserDto?>(AuthActionResult.ErrorLoggedInUser));

        return Ok(ResponseHelper.Success(AuthActionResult.SuccessGotCurrentUser, userDto));
    }
}
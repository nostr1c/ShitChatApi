using api.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using FluentValidation;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly IAuthService _accountService;
        private readonly IServiceProvider _serviceProvider;

        public AuthController
        (
            IConfiguration config,
            IAuthService accountService,
            IServiceProvider serviceProvider
        )
        {
            _config = config;
            _accountService = accountService;
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

                response.Message = "Validation failed.";
                return BadRequest(response);
            }

            var user = await _accountService.RegisterUserAsync(request);
            if (user == null)
            {
                return BadRequest("Internal server error");
            }

            CreateUserDto dto = new CreateUserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            };

            response.Data = dto;
            response.Message = "User created successfully.";

            return Ok(response);
        }

        /// <summary>
        /// Login a user
        /// </summary>
        [HttpPost("Login")]
        public async Task<ActionResult<GenericResponse<LoginUserDto>>> Login([FromBody] LoginUserRequest request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<LoginUserRequest>>();
            var validationResult = await validator.ValidateAsync(request);
            var response = new GenericResponse<LoginUserDto>();

            if (!validationResult.IsValid)
            {
                response.Errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(y => y.ErrorMessage).ToList()
                    );

                response.Message = "Validation failed.";
                return BadRequest(response);
            }

            var (success, message, userDto) = await _accountService.LoginUserAsync(request);

            if (!success)
            {
                response.Errors.Add("Authentication", new List<string> { message });
                return BadRequest(response);
            }

            response.Data = userDto;

            return Ok(response);
        }
    }
}

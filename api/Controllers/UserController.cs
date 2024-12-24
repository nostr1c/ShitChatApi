using api.Data;
using api.Data.Models;
using api.Models;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<User> _userManager;

        public UserController
        (
            AppDbContext dbContext,
            IUserService userService,
            IServiceProvider serviceProvider,
            UserManager<User> userManager
        )
        {
            _dbContext = dbContext;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
        }

        /// <summary>
        /// Get specific user by Guid
        /// </summary>
        [HttpGet("{guid}")]
        public async Task<ActionResult<GenericResponse<UserDto>>> GetUserByGuid(string guid)
        {
            var response = new GenericResponse<UserDto>();

            var (success, user) = await _userService.GetUserByGuidAsync(guid);

            if (!success || user == null)
            {
                response.Errors.Add("Error", new List<string> { "ErrorUserNotFound" });
                return NotFound(response);
            }

            var dto = new UserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email,
                Avatar = user.AvatarUri,
                CreatedAt = user.CreatedAt
            };
           
            response.Data = dto;

            return Ok(response);
        }

        /// <summary>
        /// Update avatar
        /// </summary>
        [HttpPut("ChangeAvatar")]
        public async Task<ActionResult<GenericResponse<string?>>> UpdateAvatar([FromBody] UpdateAvatarRequest request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateAvatarRequest>>();
            var validationResult = await validator.ValidateAsync(request);
            var response = new GenericResponse<string?>();

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

            var (success, avatarUri) = await _userService.UpdateAvatarAsync(request);

            if (!success || avatarUri == null)
            {
                response.Errors.Add("Error", new List<string> { "ErrorUpdatingAvatar" });
                return BadRequest(response);
            }

            response.Data = avatarUri;

            return Ok(response);
        }
    }
}

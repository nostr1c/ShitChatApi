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
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;
        private readonly UserManager<User> _userManager;
        private readonly IGroupService _groupService;

        public UserController
        (
            AppDbContext dbContext,
            IUserService userService,
            IServiceProvider serviceProvider,
            UserManager<User> userManager,
            IGroupService groupService
        )
        {
            _dbContext = dbContext;
            _userService = userService;
            _serviceProvider = serviceProvider;
            _userManager = userManager;
            _groupService = groupService;
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

        /// <summary>
        /// Get user connections
        /// </summary>
        [HttpGet("Connections")]
        public async Task<ActionResult<GenericResponse<List<ConnectionDto>>>> GetConnections()
        {
            var response = new GenericResponse<List<ConnectionDto>>();
            // TODO: Change to groupservice
            var connections = await _userService.GetConnectionsAsync();

            response.Data = connections;

            return Ok(response);
        }

        /// <summary>
        /// Get user groups
        /// </summary>
        [HttpGet("Groups")]
        public async Task<ActionResult<GenericResponse<List<GroupDto>>>> GetGroups()
        {
            var response = new GenericResponse<List<GroupDto>>();
            var groups = await _groupService.GetUserGroupsAsync();

            response.Data = groups;

            return Ok(response);
        }
    }
}

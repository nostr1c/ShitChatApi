using api.Models;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;
using Azure.Core;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class GroupController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IGroupService _groupService;
        private readonly ILogger<GroupController> _logger;

        public GroupController
        (
            IServiceProvider serviceProvider,
            IGroupService groupService,
            ILogger<GroupController> logger
        )
        {
            _serviceProvider = serviceProvider;
            _groupService = groupService;
            _logger = logger;
        }

        /// <summary>
        /// Create group
        /// </summary>
        [HttpPost("Create")]
        public async Task<ActionResult<GenericResponse<GroupDto>>> CreateGroup([FromBody] CreateGroupRequest request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateGroupRequest>>();
            var validationResult = await validator.ValidateAsync(request);
            var response = new GenericResponse<GroupDto>();

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

            var group = await _groupService.CreateGroupAsync(request);

            response.Data = group;

            return Ok(response);
        }

        /// <summary>
        /// Add member to group
        /// </summary>
        [HttpPost("{groupGuid}/members/add")]
        public async Task<ActionResult<GenericResponse<UserDto>>> AddUserToGroup(Guid groupGuid, [FromBody] string userId)
        {
            var response = new GenericResponse<UserDto>();

            var (success, message, userDto) = await _groupService.AddUserToGroupAsync(groupGuid, userId);

            if (!success || userDto == null)
            {
                response.Errors.Add("Error", new List<string> { message });
                return BadRequest(response);
            }

            response.Message = message;
            response.Data = userDto;

            return Ok(response);
        }

        /// <summary>
        /// List group members
        /// </summary>
        [HttpGet("{groupGuid}/members")]
        public async Task<ActionResult<GenericResponse<IEnumerable<UserDto>>>> GetGroupMembers(Guid groupGuid)
        {
            var response = new GenericResponse<IEnumerable<UserDto>>();

            var (success, message, users) = await _groupService.GetGroupMembersAsync(groupGuid);

            if (!success)
            {
                response.Errors.Add("Error", new List<string> { message });
                return BadRequest(response);
            }

            response.Message = message;
            response.Data = users;

            return Ok(response);
        }
    }
}

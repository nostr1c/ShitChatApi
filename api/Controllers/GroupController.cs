using api.Models;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;
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

        public GroupController
        (
            IServiceProvider serviceProvider,
            IGroupService groupService
        )
        {
            _serviceProvider = serviceProvider;
            _groupService = groupService;
        }

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
            return Ok(group);
        }
    }
}

using api.Data;
using api.Data.Models;
using api.Models;
using api.Models.Dtos;
using api.Models.Requests;
using api.Services.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly IUserService _userService;
        private readonly IServiceProvider _serviceProvider;

        public UserController
        (
            AppDbContext dbContext,
            IUserService userService,
            IServiceProvider serviceProvider
        )
        {
            _dbContext = dbContext;
            _userService = userService;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("{guid}")]
        public async Task<ActionResult<GenericResponse<UserDto>>> GetUserByGuid(string guid)
        {
            var response = new GenericResponse<UserDto>();

            var (success, user) = await _userService.GetUserByGuidAsync(guid);

            if (!success || user == null)
            {
                response.Errors.Add("Not found", new List<string> { "User not found." });
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

        [HttpPut("{guid}")]
        public async Task<ActionResult<GenericResponse<UpdateUserDto>>> UpdateUserByGuid([FromBody] UpdateUserRequest request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateUserRequest>>();
            var validationResult = await validator.ValidateAsync(request);
            var response = new GenericResponse<UpdateUserDto>();

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


            var (success, user) = await _userService.UpdateUserByGuidAsync(request);

            if (!success || user == null)
            {
                response.Errors.Add("Error", new List<string> { "Error updating user." });
                return BadRequest(response);
            }

            var dto = new UpdateUserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Avatar = user.AvatarUri,
            };

            response.Data = dto;

            return Ok(response);
        }
    }
}

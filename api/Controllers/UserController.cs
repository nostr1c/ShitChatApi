using api.Models;
using api.Models.Reponses;
using api.Services;
using Microsoft.AspNetCore.Mvc;
using api.Dtos;
using FluentValidation;

namespace api.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly UserService _userService;
        private readonly IServiceProvider _serviceProvider;

        public UserController
        (
            ILogger<UserController> logger,
            UserService userService,
            IServiceProvider serviceProvider
        )
        {
            _logger = logger;
            _userService = userService;
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// Get all users.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<GenericResponse<IEnumerable<User>>>> GetUsers()
        {
            var response = new GenericResponse<IEnumerable<User>>();
            var users = await _userService.GetUsersAsync();

            response.Data = users;
            return Ok(response);
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        [HttpGet("{userId:int}")]
        public async Task<ActionResult<GenericResponse<User>>> GetUserById(int userId)
        {
            var response = new GenericResponse<User>();
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                response.Message = "User not found.";

                return NotFound(response);
            }

            response.Data = user;

            return Ok(response);
        }

        /// <summary>
        /// Create user.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<GenericResponse<User>>> CreateUser([FromBody] CreateUserRequest createUserRequest)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateUserRequest>>();
            var validationResult = await validator.ValidateAsync(createUserRequest);
            var response = new GenericResponse<User>();

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

            var createdUser = await _userService.CreateUserAsync(createUserRequest);

            response.Data = createdUser;
            response.Message = "User created successfully.";

            return Ok(response);
        }


        /// <summary>
        /// Update user.
        /// </summary>
        [HttpPut]
        public async Task<ActionResult<GenericResponse<User>>> UpdateUser([FromBody] UpdateUserRequest updateUserRequest)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<UpdateUserRequest>>();
            var validationResult = await validator.ValidateAsync(updateUserRequest);
            var response = new GenericResponse<User>();

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

            var createdUser = await _userService.UpdateUserAsync(updateUserRequest);

            response.Data = createdUser;
            response.Message = "User updated successfully.";

            return Ok(response);
        }
    }
}

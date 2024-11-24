using api.Models;
using api.Models.Reponses;
using api.Services;
using api.Repositories.Exceptions;
using Microsoft.AspNetCore.Mvc;
using api.Dtos;
using api.Repositories;
using FluentValidation;
using FluentValidation.Results;
using api.Helpers;

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
            try
            {
                var users = await _userService.GetUsersAsync();

                return Ok(new GenericResponse<IEnumerable<User>>()
                {
                    Data = users
                });
            }
            catch (RepositoryException repoEx)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse<IEnumerable<User>>
                {
                    Message = repoEx.Message,
                });
            }
        }

        /// <summary>
        /// Get user by id
        /// </summary>
        [HttpGet("{userId:int}")]
        public async Task<ActionResult<GenericResponse<User>>> GetUserById(int userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user == null)
            {
                return NotFound(new GenericResponse<IEnumerable<User>>()
                {
                    Message = "User not found."
                });
            }

            return Ok(new GenericResponse<User>()
            {
                Data = user
            });
        }

        /// <summary>
        /// Create user.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<GenericResponse<User>>> CreateUser([FromBody] CreateUserRequest createUserRequest)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateUserRequest>>();
            ValidationResult validationResult = await validator.ValidateAsync(createUserRequest);
            var response = new GenericResponse<User>();

            if (!validationResult.IsValid)
            {
                response.Errors = validationResult.Errors
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        x => x.Key,
                        x => x.Select(x => x.ErrorMessage).ToList()
                    );

                response.Message = "Validation failed.";
                return BadRequest(response);
            }

            try
            {
                var createdUser = await _userService.CreateUserAsync(createUserRequest);

                response.Data = createdUser;
                response.Message = "User created successfully.";
                return Ok(response);
            }
            catch (RepositoryException repoEx)
            {
                response.Errors.Add("Exception", new List<string> { repoEx.Message });
                response.Message = repoEx.Message;
                return StatusCode(500, response);
            }
        }
    }
}

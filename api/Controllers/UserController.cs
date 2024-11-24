using api.Models;
using api.Models.Reponses;
using api.Services;
using api.Repositories.Exceptions;
using Microsoft.AspNetCore.Mvc;

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
                return StatusCode(StatusCodes.Status500InternalServerError, new GenericResponse
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
                return NotFound(new GenericResponse()
                {
                    Message = "User not found."
                });
            }

            return Ok(new GenericResponse<User>()
            {
                Data = user
            });
        }
    }
}

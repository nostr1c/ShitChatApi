using api.Data;
using api.Data.Models;
using api.Models;
using api.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/v1/[controller]")]
    public class ConnectionController : ControllerBase
    {
        private readonly ILogger<ConnectionController> _logger;
        private readonly IConnectionService _connectionService;
        private readonly UserManager<User> _userManager;

        public ConnectionController
        (
            ILogger<ConnectionController> logger,
            IConnectionService connectionService,
            UserManager<User> userManager
        )
        {
            _logger = logger;
            _connectionService = connectionService;
            _userManager = userManager;
        }

        /// <summary>
        /// Create a new connection request
        /// </summary>
        [HttpPost("Add")]
        public async Task<ActionResult<GenericResponse<string>>> CreateConnection([FromBody] string friendId)
        {
            var response = new GenericResponse<string>();

            var (success, message) = await _connectionService.CreateConnectionAsync(User.Identity.Name, friendId);

            if (!success)
            {
                response.Errors.Add("ErrorCreatingConnection", new List<string> { message });
                return BadRequest(response);
            }

            response.Message = message;

            return Ok(response);
        }

        /// <summary>
        /// Accept a friend request.
        /// </summary>
        [HttpPut("Accept")]
        public async Task<ActionResult<GenericResponse<string>>> AcceptConnection([FromBody] string friendId)
        {
            var response = new GenericResponse<string>();

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return BadRequest("error");

            var (success, message) = await _connectionService.AcceptConnectionAsync(user.Id, friendId);

            if (!success)
            {
                response.Errors.Add("Error", new List<string> { message });
                return BadRequest(response);
            }

            response.Message = message;

            return Ok(response);
        }


        /// <summary>
        /// Delete a friend
        /// </summary>
        [HttpDelete("Delete")]
        public async Task<ActionResult<GenericResponse<string>>> DeleteConnection([FromBody] string friendId)
        {
            var response = new GenericResponse<string>();

            var user = await _userManager.GetUserAsync(HttpContext.User);
            if (user == null)
                return BadRequest("error");

            var (success, message) = await _connectionService.DeleteConnectionAsync(user.Id, friendId);

            if (!success)
            {
                response.Errors.Add("Error", new List<string> { message });
                return BadRequest(response);
            }

            response.Message = message;

            return Ok(response);
        }
    }
}

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
    [Route("api/[controller]")]
    public class ConnectionController : ControllerBase
    {
        private readonly ILogger<ConnectionController> _logger;
        private readonly IConnectionService _connectionService; 

        public ConnectionController
        (
            ILogger<ConnectionController> logger,
            IConnectionService connectionService
        )
        {
            _logger = logger;
            _connectionService = connectionService;
        }

        [HttpPost("Add")]
        public async Task<ActionResult<GenericResponse<string>>> CreateConnection([FromBody] string friendId)
        {
            var response = new GenericResponse<string>();

            var (success, message) = await _connectionService.CreateConnectionAsync(User.Identity.Name, friendId);

            if (!success)
            {
                response.Errors.Add("User", new List<string> { message });
                return BadRequest(response);
            }

            response.Message = message;

            return Ok(response);
        }
    }
}

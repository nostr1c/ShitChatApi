using api.Data;
using api.Data.Models;
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
        private readonly UserManager<User> _userManager;
        private readonly AppDbContext _appDbContext;

        public ConnectionController
        (
            ILogger<ConnectionController> logger,
            UserManager<User> userManager,
            AppDbContext appDbContext
        )
        {
            _logger = logger;
            _userManager = userManager;
            _appDbContext = appDbContext;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> CreateConnection([FromBody] string friendId)
        {

            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest();
            }

            var friend = await _userManager.FindByIdAsync(friendId);
            if (friend == null)
            {
                return BadRequest();
            }

            Connection connection = new Connection
            {
                UserId = user.Id,
                FriendId = friendId
            };

            await _appDbContext.Connections.AddAsync(connection);
            await _appDbContext.SaveChangesAsync();

            return Ok("Connection added");
        }
    }
}

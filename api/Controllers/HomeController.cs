using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
{
    [Authorize(AuthenticationSchemes = "Bearer")]
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController
        (
            ILogger<HomeController> logger
        )
        {
            _logger = logger;
        }

        [HttpGet]
        public IActionResult GetValues()
        {
            return Ok(new { Value = "OMG!!! IT WORKS!" });
        }
    }
}

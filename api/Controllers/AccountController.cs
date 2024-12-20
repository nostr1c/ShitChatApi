using api.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using api.Models;
using api.Helpers;
using api.Services;
using FluentValidation;

namespace api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _config;
        private readonly IAccountService _accountService;
        private readonly IServiceProvider _serviceProvider;

        public AccountController
        (
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            IConfiguration config,
            IAccountService accountService,
            IServiceProvider serviceProvider
        )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _config = config;
            _accountService = accountService;
            _serviceProvider = serviceProvider;
        }

        [HttpGet("All")]
        public async Task<ActionResult<ICollection<User>>> GetAll()
        {
            var response = _accountService.GetAll();
            return Ok(response);
        }


        [HttpPost("Register")]
        public async Task<ActionResult<GenericResponse<CreateUserDto>>> Register([FromBody] CreateUserRequest request)
        {
            var validator = _serviceProvider.GetRequiredService<IValidator<CreateUserRequest>>();
            var validationResult = await validator.ValidateAsync(request);
            var response = new GenericResponse<CreateUserDto>();

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

            var user = await _accountService.RegisterUserAsync(request);
            if (user == null)
            {
                return BadRequest("Internal server error");
            }

            CreateUserDto userDto = new CreateUserDto
            {
                Id = user.Id,
                Username = user.UserName,
                Email = user.Email
            };

            response.Data = userDto;
            response.Message = "User created successfully.";

            return Ok(response);
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] RequestLogin request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return Unauthorized("Invalid Email or Password");
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            if (!result.Succeeded)
            {
                return Unauthorized("Invalid Email or Password");
            }

            var jwtKey = _config["Jwt:Key"];
            var jwtIssuer = _config["Jwt:Issuer"];
            var token = JwtHelper.GenerateJwtToken(user.Id, user.UserName, jwtKey, jwtIssuer);

            return Ok(new { token });
        }
    }
}

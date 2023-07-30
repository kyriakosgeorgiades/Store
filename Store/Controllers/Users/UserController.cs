using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Auth;
using Store.Entities;
using Store.Interface;
using Store.Models.Users.Request;
using Store.Models.Users.Response;

namespace Store.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UserController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UserController(IUserRepository userRepository, ILogger<UserController> logger, IConfiguration configuration, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); 
        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPost("Login")]
    public async Task<IActionResult> Login(LoginModelRequest model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userRepository.GetUserByUserNameOrEmail(model.UserNameOrEmail);
            if (user != null)
            {
                if (!Authentication.VerifyPassword(model.Password, user.Password))
                {
                    _logger.LogWarning($"Invalid login attempt for : {model.UserNameOrEmail}");
                    return Unauthorized(new { message = "Invalid login credentials" });
                }

                var token = Authentication.GenerateJwtToken(user, _configuration);
                _logger.LogInformation("Token initialized!");

                return Ok(new LoginModelResponse { Jwt = token });
            }
            else
            {
                // User not found
                _logger.LogWarning($"Invalid login attempt for : {model.UserNameOrEmail}.");
                return NotFound(new { message = "User not found" });
            }
        }

        // Model validation failed
        _logger.LogWarning($"Login attempt with invalid model state.");
        return BadRequest(new { message = "Bad request" });
    }


    [HttpPost("Register")]
    public async Task<IActionResult> Register(RegisterModelRequest model)
    {
        if (ModelState.IsValid)
        {
            if (await _userRepository.UsernameExists(model.UserName))
            {
                return BadRequest(new { message = "Username already exists." });
            }

            if (await _userRepository.EmailExists(model.Email))
            {
                return BadRequest(new { message = "Email already in use." });
            }
            model.Password = Authentication.HashPassword(model.Password);
            await _userRepository.RegisterUser(_mapper.Map<User>(model));
            return Created("", null);
        }

        // Model validation failed
        _logger.LogWarning($"Registration attempt with invalid model state.");
        return BadRequest(new { message = "Bad request" });
    }
}


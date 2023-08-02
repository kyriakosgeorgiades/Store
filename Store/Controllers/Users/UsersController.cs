using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Store.Auth;
using Store.Entities;
using Store.Examples.Users.Request;
using Store.Interface;
using Store.Models.Users.Request;
using Store.Models.Users.Response;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Store.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public UsersController(IUserRepository userRepository, ILogger<UsersController> logger, IConfiguration configuration, IMapper mapper)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository)); 
        _logger = logger ?? throw new ArgumentNullException(nameof(logger)); 
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    [HttpPost("Login")]
    /// <summary>
    /// Logs the user in and returns a JWT for authenticated requests.
    /// </summary>
    /// <param name="model">The login request model containing user credentials.</param>
    /// <returns>A JWT on successful login; otherwise, an error message.</returns>
    [SwaggerOperation(Summary = "Logs the user in")]
    [SwaggerRequestExample(typeof(LoginModelRequest), typeof(LoginModelRequestExample))]
    [SwaggerResponse(200, "Login successful", Type = typeof(LoginModelResponse))]
    [SwaggerResponse(401, "Invalid login credentials", Type = typeof(string))]
    [SwaggerResponse(404, "User not found", Type = typeof(string))]
    [SwaggerResponse(400, "Bad request due to invalid model state", Type = typeof(string))]
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
    /// <summary>
    /// Registers a new user to the system.
    /// </summary>
    /// <param name="model">The registration request model containing user details.</param>
    /// <returns>A success status on successful registration; otherwise, an error message.</returns>
    [SwaggerOperation(Summary = "Registers a new user")]
    [SwaggerRequestExample(typeof(RegisterModelRequest), typeof(RegisterModelRequestExample))]
    [SwaggerResponse(201, "User registered successfully")]
    [SwaggerResponse(400, "Bad request due to username/email in use or invalid model state", Type = typeof(string))]
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


using Microsoft.AspNetCore.Mvc;
using Store.Auth;
using Store.Domains.Interfaces.Users;
using Store.Exceptions;
using Store.Models.Users.Request;
using Store.Models.Users.Response;
using Swashbuckle.AspNetCore.Annotations;

namespace Store.Controllers.Users;

[Route("api/[controller]")]
[ApiController]
public class UsersController : ControllerBase
{
    private readonly ILogger<UsersController> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUserDomainService _userDomainService;

    public UsersController(ILogger<UsersController> logger, IConfiguration configuration, IUserDomainService userDomainService)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        _userDomainService = userDomainService ?? throw new ArgumentNullException(nameof(userDomainService));
    }

    /// <summary>
    /// Authenticates the user based on the provided credentials and returns a JWT token.
    /// </summary>
    /// <param name="model">The login details of the user.</param>
    /// <returns>Details of authenticated user along with a JWT token.</returns>
    [HttpPost("Login")]
    [SwaggerOperation(Summary = "Logs the user in and returns a JWT token.")]
    [SwaggerResponse(200, "Successfully authenticated and token returned.", Type = typeof(LoginModelResponse))]
    [SwaggerResponse(400, "Invalid request or authentication failed.", Type = typeof(string))]
    public async Task<IActionResult> Login(LoginModelRequest model)
    {
        if (!ModelState.IsValid)
            throw new BadRequestException("Invalid request.");

        var user = await _userDomainService.AuthenticateAsync(model);
        var token = Authentication.GenerateJwtToken(user, _configuration);

        _logger.LogInformation("Token initialized!");

        return Ok(new LoginModelResponse
        {
            UserId = user.UserId,
            UserEmail = user.Email,
            UserName = user.UserName,
            Jwt = token
        });
    }

    /// <summary>
    /// Registers a new user in the system.
    /// </summary>
    /// <param name="model">The registration details of the new user.</param>
    /// <returns>Returns 201 Created upon successful registration.</returns>
    [HttpPost("Register")]
    [SwaggerOperation(Summary = "Registers a new user.")]
    [SwaggerResponse(201, "User successfully registered.")]
    [SwaggerResponse(400, "Invalid registration data or user already exists.", Type = typeof(string))]
    public async Task<IActionResult> Register(RegisterModelRequest model)
    {
 
        await _userDomainService.RegisterAsync(model);

        return Created("", null);
    }

    /// <summary>
    /// Validates the provided JWT token.
    /// </summary>
    /// <returns>Confirms the validity of the token.</returns>
    [HttpGet("ValidateToken")]
    [SwaggerOperation(Summary = "Validates the provided JWT token")]
    [SwaggerResponse(200, "Token is valid")]
    [SwaggerResponse(401, "Token is invalid or expired", Type = typeof(string))]
    public IActionResult ValidateToken()
    {
        var authHeader = this.Request.Headers["Authorization"].ToString();

        if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            throw new UnauthorizedAccessException("No JWT token provided.");

        var token = authHeader.Split(" ")[1];
        var isValid = Authentication.ValidateJwtToken(token, _configuration);

        if (!isValid)
            throw new UnauthorizedAccessException("Token is invalid or expired.");

        return Ok(new { message = "Token is valid." });
    }
}

using Microsoft.EntityFrameworkCore;
using Store.Context;
using Store.Entities;
using Store.IRepository;

namespace Store.Repository;

/// <summary>
/// Repository implementation for handling User entities.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserRepository"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    /// <param name="logger">The logger for error reporting.</param>
    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }


    /// <summary>
    /// Retrieves a User entity from the database by its UserName or Email.
    /// </summary>
    /// <param name="identifier">The UserName or Email of the User.</param>
    /// <returns>The User entity with the specified UserName or Email, or null if not found.</returns>
    public async Task<User> GetUserByUserNameOrEmail(string identifier)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserName == identifier || x.Email == identifier);
    }


    /// <summary>
    /// Registers a new User in the database.
    /// </summary>
    /// <param name="user">The User entity to be registered.</param>
    /// <returns>True if the User registration is successful, otherwise false.</returns>
    public async Task<bool> RegisterUser(User user)
    {
        try
        {
            // Add the User entity to the context's Users DbSet.
            await _context.Users.AddAsync(user);

            // Save the changes to the database asynchronously.
            await _context.SaveChangesAsync();

            // Log the successful User registration.
            _logger.LogInformation("User Register");

            // Return true to indicate successful registration.
            return true;
        }
        catch (Exception ex)
        {
            // Log the exception that occurred during registration.
            _logger.LogError($"Something went wrong during registration: {ex.Message}", ex);

            // Return false to indicate registration failure.
            return false;
        }
    }



    /// <summary>
    /// Checks if a User with the specified UserName exists in the database.
    /// </summary>
    /// <param name="username">The UserName to check.</param>
    /// <returns>True if a User with the specified UserName exists, otherwise false.</returns>
    public async Task<bool> UsernameExists(string username)
    {
        // Check if any User entity in the Users DbSet has a matching UserName.
        return await _context.Users.AnyAsync(u => u.UserName == username);
    }

    /// <summary>
    /// Checks if a User with the specified Email exists in the database.
    /// </summary>
    /// <param name="email">The Email to check.</param>
    /// <returns>True if a User with the specified Email exists, otherwise false.</returns>
    public async Task<bool> EmailExists(string email)
    {
        // Check if any User entity in the Users DbSet has a matching Email.
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
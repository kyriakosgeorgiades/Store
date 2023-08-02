using Microsoft.EntityFrameworkCore;
using Store.Context;
using Store.Controllers.Users;
using Store.Entities;
using Store.Interface;
using Store.Models.Users.Request;

namespace Store.Repository;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<UserRepository> _logger;

    public UserRepository(AppDbContext context, ILogger<UserRepository> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<User> GetUserByUserNameOrEmail(string identifier)
    {
        return await _context.Users.FirstOrDefaultAsync(x => x.UserName == identifier || x.Email == identifier);
    }

     public async Task<bool> RegisterUser(User user)
    {
        try
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            _logger.LogInformation("User Register");
            return true;
        }
        catch (Exception ex) {
            _logger.LogError($"Something went wrong during registration: {ex.Message}", ex);
            return false;
        }

    }

    public async Task<bool> UsernameExists(string username)
    {
        return await _context.Users.AnyAsync(u => u.UserName == username);
    }

    public async Task<bool> EmailExists(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}

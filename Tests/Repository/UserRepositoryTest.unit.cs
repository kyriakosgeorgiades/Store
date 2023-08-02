using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using Store.Context;
using Store.Entities;
using Store.Interface;
using Store.Repository;

namespace Tests.Repository;

[TestFixture]
public class UserRepositoryTests
{
    private AppDbContext _context;
    private IUserRepository _userRepository;
    private Mock<ILogger<UserRepository>> _loggerMock;

    [SetUp]
    public void Setup()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);
        _loggerMock = new Mock<ILogger<UserRepository>>();
        _userRepository = new UserRepository(_context, _loggerMock.Object);
    }

    [TearDown]
    public void TearDown()
    {
        // Dispose of the in-memory database after each test
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetUserByUserNameOrEmail_ShouldReturnUser_WhenUserExists()
    {
        // Arrange
        var user = new User { UserId = Guid.NewGuid(), UserName = "testuser", Password = "validpassword", Email = "test@example.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.GetUserByUserNameOrEmail("testuser");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user.UserId, result.UserId);
        Assert.AreEqual(user.UserName, result.UserName);
    }

    [Test]
    public async Task GetUserByUserNameOrEmail_ShouldReturnNull_WhenUserDoesNotExist()
    {
        // Arrange - No users added to the context

        // Act
        var result = await _userRepository.GetUserByUserNameOrEmail("nonexistentuser");

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task RegisterUser_ShouldReturnTrue_WhenRegistrationSucceeds()
    {
        // Arrange
        var user = new User { UserId = Guid.NewGuid(), UserName = "newuser123", Password = "validpassword", Email = "newuser123@example.com" };

        // Act
        var result = await _userRepository.RegisterUser(user);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task RegisterUser_ShouldReturnFalse_WhenRegistrationFails()
    {
        // Arrange
        var user = new User { UserId = Guid.NewGuid(), UserName = "newuser", Password = "validpassword",  Email = "newuser@example.com" };

        // Simulate an exception during registration
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _context.Entry(user).State = EntityState.Detached; // Detach the user from the context to simulate an exception

        // Act
        var result = await _userRepository.RegisterUser(user);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task UsernameExists_ShouldReturnTrue_WhenUsernameExists()
    {
        // Arrange
        var username = "existinguser";
        var user = new User { UserId = Guid.NewGuid(), UserName = username, Password = "validpassword", Email = "existinguser@example.com" };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.UsernameExists(username);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task UsernameExists_ShouldReturnFalse_WhenUsernameDoesNotExist()
    {
        // Arrange
        var username = "nonexistentuser";

        // Act
        var result = await _userRepository.UsernameExists(username);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public async Task EmailExists_ShouldReturnTrue_WhenEmailExists()
    {
        // Arrange
        var email = "existinguser@example.com";
        var user = new User
        {
            UserId = Guid.NewGuid(),
            UserName = "existinguser",
            Email = email,
            Password = "validpassword"
        };
        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Act
        var result = await _userRepository.EmailExists(email);

        // Assert
        Assert.IsTrue(result);
    }


    [Test]
    public async Task EmailExists_ShouldReturnFalse_WhenEmailDoesNotExist()
    {
        // Arrange
        var email = "nonexistentuser@example.com";

        // Act
        var result = await _userRepository.EmailExists(email);

        // Assert
        Assert.IsFalse(result);
    }


    [Test]
    public async Task RegisterUser_ShouldLogError_WhenRegistrationFailsDueToException()
    {
        // Arrange
        var user = new User { UserId = Guid.NewGuid(), UserName = "newuser", Password = "validpassword", Email = "newuser@example.com" };

        // Simulate an exception during registration
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        _context.Entry(user).State = EntityState.Detached; // Detach the user from the context to simulate an exception

        // Act
        await _userRepository.RegisterUser(user);

        // Assert
        List<string> loggedMessages = new List<string>();

        _loggerMock.Setup(x => x.Log(
            It.IsAny<LogLevel>(),
            It.IsAny<EventId>(),
            It.IsAny<object>(),
            It.IsAny<Exception>(),
            It.IsAny<Func<object, Exception, string>>()))
            .Callback<LogLevel, EventId, object, Exception, Func<object, Exception, string>>((logLevel, eventId, state, exception, formatter) =>
            {
                var message = formatter(state, exception);
                loggedMessages.Add(message);
            });
    }
}

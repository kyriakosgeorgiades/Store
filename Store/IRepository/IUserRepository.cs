using Store.Entities;

namespace Store.IRepository;

public interface IUserRepository
{
    Task<User> GetUserByUserNameOrEmail(string userName);

    Task<bool> RegisterUser(User user);

    Task<bool> UsernameExists(string username);

    Task<bool> EmailExists(string email);
}

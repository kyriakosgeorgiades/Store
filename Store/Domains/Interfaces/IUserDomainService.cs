using Store.Entities;
using Store.Models.Users.Request;

namespace Store.Domains.Interfaces.Users;

public interface IUserDomainService
{
    Task<User?> AuthenticateAsync(LoginModelRequest model);

    Task RegisterAsync(RegisterModelRequest model);

    bool ValidateToken(string token);
}


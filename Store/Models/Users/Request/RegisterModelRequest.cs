namespace Store.Models.Users.Request;

public class RegisterModelRequest
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public string Password { get; set; }
}

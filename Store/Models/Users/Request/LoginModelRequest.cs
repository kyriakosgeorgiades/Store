namespace Store.Models.Users.Request;

public class LoginModelRequest
{
    public string UserNameOrEmail { get; set; }

    public string Password { get; set; }
}

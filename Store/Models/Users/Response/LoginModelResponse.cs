namespace Store.Models.Users.Response;

public class LoginModelResponse
{
    public Guid UserId { get; set; }

    public string UserName { get; set; }

    public string UserEmail { get; set; }

    public string Jwt { get; set; }
}

namespace Store.Models.Users;

public record UserModel
{
    public Guid userId { get; set; }
    public string password { get; set; }
}

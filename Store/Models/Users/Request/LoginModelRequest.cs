using System.ComponentModel.DataAnnotations;

namespace Store.Models.Users.Request;

public class LoginModelRequest
{
    [Required(ErrorMessage = "Username or Email is required.")]
    [StringLength(255, ErrorMessage = "Username or Email should not exceed 255 characters.")]
    public string UserNameOrEmail { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password should be between 6 and 100 characters.")]
    public string Password { get; set; }
}

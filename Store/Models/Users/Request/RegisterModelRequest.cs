using System.ComponentModel.DataAnnotations;

namespace Store.Models.Users.Request;

public class RegisterModelRequest
{
    [Required(ErrorMessage = "Username is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Username should be between 3 and 255 characters.")]
    public string UserName { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Invalid email format.")]
    [StringLength(255, ErrorMessage = "Email should not exceed 255 characters.")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Password is required.")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Password should be between 6 and 100 characters.")]
    public string Password { get; set; }
}

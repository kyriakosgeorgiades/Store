using System.ComponentModel.DataAnnotations;

namespace Store.Models.Books.Request;

public class SaveBookRequest
{
    public Guid? BookId { get; set; }

    [Required(ErrorMessage = "Book name is required.")]
    [StringLength(255, MinimumLength = 3, ErrorMessage = "Book name should be between 3 and 255 characters.")]
    public string BookName { get; set; }

    public AuthorDto Author { get; set; }  

    [Required(ErrorMessage = "ISBN is required.")]
    public string ISBN { get; set; }

    [Required(ErrorMessage = "Publication year is required.")]
    public DateTime PublicationYear { get; set; }
}

public class AuthorDto
{
    public Guid? AuthorId { get; set; }

    public string AuthorName { get; set; }
}

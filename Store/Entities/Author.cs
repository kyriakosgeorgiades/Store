using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Store.Entities;

public class Author
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid AuthorId { get; set; }

    [Required]
    [MaxLength(150)]
    public string AuthorName { get; set; }

    public IEnumerable<Book> Books { get; set; } = new List<Book>();
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Store.Entities;

public class Book
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid BookId { get; set; }

    [Required]
    [MaxLength(250)]
    public string BookName { get; set; }

    [Required]
    public string BookUrl { get; set; }

    [Required]
    public DateTime PublicationYear { get; set; }

    [Required]
    public string ISBN { get; set; }

    [ForeignKey("Author")]
    public Guid AuthId { get; set; }

    public Author Author { get; set; }
}

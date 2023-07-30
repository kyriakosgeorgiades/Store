namespace Store.Models.Books;

public class BookModel
{
    public Guid BookId { get; set; }

    public string BookName { get; set; }

    public string BookUrl { get; set; }

    public Guid AuthId { get; set; }
}

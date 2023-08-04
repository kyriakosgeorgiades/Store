using Swashbuckle.AspNetCore.Annotations;

namespace Store.Models.Books.Response;

public class BookSearchResponse
{
    public Guid BookId { get; set; }
    public string BookName { get; set; }
    public string BookUrl { get; set; }
    public string ISBN { get; set; }
    public DateTime PublicationYear { get; set; }
    public AuthorSearchResponse Author { get; set; }
}

public class BooksWrapperResponse
{
    public IEnumerable<BookSearchResponse> Books { get; set; }
}


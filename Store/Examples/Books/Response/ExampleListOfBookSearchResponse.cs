namespace Store.Examples.Books.Response;

using Store.Models.Books.Response;
using Swashbuckle.AspNetCore.Filters;
using System.Collections.Generic;

public class ExampleListOfBookSearchResponse : IExamplesProvider<IEnumerable<BookSearchResponse>>
{
    public IEnumerable<BookSearchResponse> GetExamples()
    {
        return new List<BookSearchResponse>
        {
            new BookSearchResponse
            {
                BookId = Guid.NewGuid(),
                BookName = "Example Book Name 1",
                BookUrl = "https://example.com/book1",
                ISBN = "978-3-16-148410-0",
                PublicationYear = new DateTime(2021, 1, 1),
                Author = new AuthorSearchResponse
                {
                    AuthorId = new Guid("A63FB91C-6E34-4F7B-B57E-08DB912DDE1A"),
                    AuthorName = "Author Name 1"
                }
            },
            new BookSearchResponse
            {
                BookId = Guid.NewGuid(),
                BookName = "Example Book Name 2",
                BookUrl = "https://example.com/book2",
                ISBN = "978-3-16-148411-0",
                PublicationYear = new DateTime(2022, 1, 1),
                Author = new AuthorSearchResponse
                {
                    AuthorId = new Guid("9A45E46C-23F3-4BDB-B57F-08DB912DDE1A"),
                    AuthorName = "Author Name 2"
                }
            }
        };
    }
}

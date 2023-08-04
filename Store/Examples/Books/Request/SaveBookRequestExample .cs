using Store.Models.Books.Request;
using Swashbuckle.AspNetCore.Filters;

namespace Store.Examples.Books.Request;

public class SaveBookRequestExample : IExamplesProvider<SaveBookRequest>
{
    public SaveBookRequest GetExamples()
    {
        return new SaveBookRequest
        {
            BookId = null, // for a new book; for updating, you'd use a real Guid.
            BookName = "Sample Book Title",
            Author = new AuthorDto
            {
                AuthorId = new Guid("9A45E46C-23F3-4BDB-B57F-08DB912DDE1A"),
                AuthorName = "Kostis",
            },
            ISBN = "978-3-16-148410-0",
            PublicationYear = new DateTime(2023, 5, 15)
        };
    }
}
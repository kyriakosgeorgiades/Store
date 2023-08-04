using Swashbuckle.AspNetCore.Filters;

namespace Store.Examples.Books.Response;

public class SaveBookResponseExample : IExamplesProvider<object>
{
    public object GetExamples()
    {
        return new
        {
            BookId = Guid.NewGuid(),
            Message = "Book successfully created."
        };
    }
}
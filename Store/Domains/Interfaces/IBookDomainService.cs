using Store.Entities;
using Store.Models.Books.Request;
using Store.Models.Books.Response;

namespace Store.Domains.Interfaces.Books;

public interface IBookDomainService
{
    Task<IEnumerable<BookSearchResponse>> GetAllBooks(string? searchTerm);

    Task<Book> SaveBook(SaveBookRequest request);

    Task<Book?> GetBookById(Guid bookId);

    Task DeleteBook(Guid bookId);
}

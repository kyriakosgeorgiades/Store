using Store.Entities;

namespace Store.IRepository
{
    public interface IBookRepository
    {
        Task<IEnumerable<Book>> GetAllBooks(string searchTerm = null);

        Task<Book> UpdateBook(Book book);

        Task<Book> AddBook(Book book);

        Task<bool> DoesISBNExist(string isbn, Guid? excludeBookId = null);

        Task<Book> GetBookById(Guid bookId);

        Task DeleteBook(Guid bookId);
    }
}

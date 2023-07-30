using Store.Entities;

namespace Store.Interface
{
    public interface IBookRepository
    {
        IEnumerable<Book> GetAllBooks();

        Task<Book> UpdateBook(Book book);

        Task<Book> AddBook(Book book);

        Task<bool> DoesISBNExist(string isbn, Guid? excludeBookId = null);
    }
}

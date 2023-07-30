using Microsoft.EntityFrameworkCore;
using Store.Context;
using Store.Entities;
using Store.Interface;

namespace Store.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;
        public BookRepository(AppDbContext context)
        { 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IEnumerable<Book> GetAllBooks()
        {
            return _context.Books.OrderBy(b => b.BookName).ToList();
        }

        public async Task<Book> AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<Book> UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }

        public async Task<bool> DoesISBNExist(string isbn, Guid? excludeBookId = null)
        {
            if (excludeBookId.HasValue)
                return await _context.Books.AnyAsync(b => b.ISBN == isbn && b.BookId != excludeBookId.Value);
            return await _context.Books.AnyAsync(b => b.ISBN == isbn);
        }

        public async Task<Book> GetBookById(Guid bookId)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
        }

        public async Task DeleteBook(Guid bookId)
        {
            var book = await _context.Books.FindAsync(bookId);
            if (book != null)
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
            }
        }
    }
}

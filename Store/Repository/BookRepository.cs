using Microsoft.EntityFrameworkCore;
using Store.Context;
using Store.Entities;
using Store.IRepository;

namespace Store.Repository
{
    /// <summary>
    /// Repository implementation for handling Book entities.
    /// </summary>
    public class BookRepository : IBookRepository
    {
        private readonly AppDbContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="BookRepository"/> class.
        /// </summary>
        /// <param name="context">The application database context.</param>
        public BookRepository(AppDbContext context)
        { 
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Retrieves a list of all books with an optional search term to filter the results.
        /// </summary>
        /// <param name="searchTerm">The optional search term.</param>
        /// <returns>A list of books matching the search term (if provided) or all books if no search term is specified.</returns>
        public async Task<IEnumerable<Book>> GetAllBooks(string searchTerm = null)
        {
            IQueryable<Book> query = _context.Books.Include(b => b.Author);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(b => b.BookName.ToLower().Contains(searchTerm)
                                      || b.Author.AuthorName.ToLower().Contains(searchTerm));
            }

            return await query.ToListAsync();
        }


        /// <summary>
        /// Adds a new Book entity to the database.
        /// </summary>
        /// <param name="book">The Book entity to be added.</param>
        /// <returns>The added Book entity.</returns>

        public async Task<Book> AddBook(Book book)
        {
            _context.Books.Add(book);
            await _context.SaveChangesAsync();
            return book;
        }


        /// <summary>
        /// Updates an existing Book entity in the database.
        /// </summary>
        /// <param name="book">The Book entity to be updated.</param>
        /// <returns>The updated Book entity.</returns>
        public async Task<Book> UpdateBook(Book book)
        {
            _context.Books.Update(book);
            await _context.SaveChangesAsync();
            return book;
        }


        /// <summary>
        /// Checks if a Book with the specified ISBN exists in the database, optionally excluding a specific Book by its ID.
        /// </summary>
        /// <param name="isbn">The ISBN of the Book to check.</param>
        /// <param name="excludeBookId">The ID of the Book to be excluded from the check (optional).</param>
        /// <returns>True if a Book with the specified ISBN exists, otherwise false.</returns>
        public async Task<bool> DoesISBNExist(string isbn, Guid? excludeBookId = null)
        {
            if (excludeBookId.HasValue)
                return await _context.Books.AnyAsync(b => b.ISBN == isbn && b.BookId != excludeBookId.Value);
            return await _context.Books.AnyAsync(b => b.ISBN == isbn);
        }


        /// <summary>
        /// Retrieves a Book entity from the database by its unique identifier (BookId).
        /// </summary>
        /// <param name="bookId">The unique identifier of the Book.</param>
        /// <returns>The Book entity with the specified BookId, or null if not found.</returns>
        public async Task<Book> GetBookById(Guid bookId)
        {
            return await _context.Books.FirstOrDefaultAsync(b => b.BookId == bookId);
        }


        /// <summary>
        /// Deletes a Book entity from the database based on its unique identifier (BookId).
        /// </summary>
        /// <param name="bookId">The unique identifier of the Book to be deleted.</param>
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

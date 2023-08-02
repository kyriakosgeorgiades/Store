using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Store.Context;
using Store.Entities;
using Store.Interface;
using Store.Repository;

namespace Tests.Repository;

[TestFixture]
public class BookRepositoryTests
{
    private AppDbContext _context;
    private IBookRepository _bookRepository;

    [SetUp]
    public void Setup()
    {
        // Use an in-memory database for testing
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb")
            .Options;

        _context = new AppDbContext(options);
        _bookRepository = new BookRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        // Dispose of the in-memory database after each test
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task GetAllBooks_ShouldReturnAllBooks_WhenNoSearchTerm()
    {
        // Arrange
        var books = new List<Book>
            {
                new Book { BookId = Guid.NewGuid(), BookName = "Book 1", BookUrl = "test.com", ISBN = "ISBN1", Author = new Author { AuthorName = "Author 1" } },
                new Book { BookId = Guid.NewGuid(), BookName = "Book 2", BookUrl = "test.com", ISBN = "ISBN2", Author = new Author { AuthorName = "Author 2" } },
            };
        _context.Books.AddRange(books);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookRepository.GetAllBooks();

        // Assert
        Assert.AreEqual(2, result.Count());
    }

    [Test]
    public async Task GetAllBooks_ShouldReturnFilteredBooks_WhenSearchTermProvided()
    {
        // Arrange
        var books = new List<Book>
            {
                new Book { BookId = Guid.NewGuid(), BookName = "Book 1",BookUrl = "test.com", ISBN = "ISBN1", Author = new Author { AuthorName = "Author 1" } },
                new Book { BookId = Guid.NewGuid(), BookName = "Book 2", BookUrl = "test.com" ,ISBN = "ISBN2", Author = new Author { AuthorName = "Author 2" } },
            };
        _context.Books.AddRange(books);
        await _context.SaveChangesAsync();

        // Act
        var result = await _bookRepository.GetAllBooks("Author 1");

        // Assert
        Assert.AreEqual(1, result.Count());
        Assert.AreEqual("Book 1", result.First().BookName);
    }

    [Test]
    public async Task AddBook_ShouldAddBookToDatabase()
    {
        // Arrange
        var book = new Book { BookId = Guid.NewGuid(), BookName = "New Book", BookUrl = "test.com", ISBN = "ISBN3", Author = new Author { AuthorName = "New Author" } };

        // Act
        await _bookRepository.AddBook(book);

        // Assert
        var addedBook = await _context.Books.FirstOrDefaultAsync(b => b.BookId == book.BookId);
        Assert.IsNotNull(addedBook);
        Assert.AreEqual(book.BookName, addedBook.BookName);
    }

    // Add similar tests for other methods (UpdateBook, DoesISBNExist, GetBookById, DeleteBook)...
}
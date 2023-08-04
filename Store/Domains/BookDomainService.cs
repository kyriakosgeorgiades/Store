using AutoMapper;
using Store.Domains.Interfaces.Books;
using Store.Entities;
using Store.Exceptions;
using Store.IRepository;
using Store.Models.Books.Request;
using Store.Models.Books.Response;

namespace Store.Domains.Books;

/// <summary>
/// Service responsible for domain-specific operations related to books.
/// </summary>
public class BookDomainService : IBookDomainService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of the <see cref="BookDomainService"/> class.
    /// </summary>
    /// <param name="bookRepository">Repository handling book data operations.</param>
    /// <param name="mapper">Mapper for converting data between objects.</param>
    public BookDomainService(IBookRepository bookRepository, IMapper mapper)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }


    /// <summary>
    /// Retrieves all books, optionally filtered by a search term.
    /// </summary>
    /// <param name="searchTerm">The term to filter books by.</param>
    /// <returns>A collection of books matching the search term.</returns>
    public async Task<IEnumerable<BookSearchResponse>> GetAllBooks(string? searchTerm)
    {
        var books = await _bookRepository.GetAllBooks(searchTerm);

        if (!books.Any())
        {
            throw new NotFoundException($"No books found for the search term: {searchTerm}.");
        }

        return _mapper.Map<IEnumerable<BookSearchResponse>>(books);
    }


    /// <summary>
    /// Persists a book to the repository, either by creating or updating.
    /// </summary>
    /// <param name="request">DTO containing book details.</param>
    /// <returns>The saved book entity.</returns>
    public async Task<Book> SaveBook(SaveBookRequest request)
    {
        var entity = _mapper.Map<Book>(request);
        entity.AuthId = entity.Author.AuthorId;
        entity.Author = null;
        entity.BookUrl = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWgel";
        if (await _bookRepository.DoesISBNExist(request.ISBN, request.BookId))
        {
            throw new ConflictException("Book with this ISBN already exists.");
        }
        if (request.BookId == null)
        {

            return await _bookRepository.AddBook(entity);
        }
        else
        {
            // Retrieve the existing book from the database
            var existingBook = await _bookRepository.GetBookById(request.BookId.Value);
            if (existingBook == null)
            {
                throw new NotFoundException("Book not found.");
            }


            existingBook.BookName = entity.BookName;
            existingBook.ISBN = entity.ISBN;
            existingBook.PublicationYear = entity.PublicationYear;
            existingBook.AuthId = entity.AuthId;
            existingBook.BookUrl = entity.BookUrl;
            existingBook.Author = entity.Author;

            await _bookRepository.UpdateBook(existingBook);
            return existingBook;
        }
    }


    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="bookId">The unique identifier of the book.</param>
    /// <returns>The book entity if found, otherwise null.</returns>
    public async Task<Book?> GetBookById(Guid bookId)
    {
        return await _bookRepository.GetBookById(bookId);
    }


    /// <summary>
    /// Deletes a book from the repository.
    /// </summary>
    /// <param name="bookId">The unique identifier of the book to delete.</param>
    public async Task DeleteBook(Guid bookId)
    {
        var book = await _bookRepository.GetBookById(bookId);

        if (book == null)
        {
            throw new NotFoundException($"Book with ID {bookId} not found.");
        }

        await _bookRepository.DeleteBook(bookId);
    }
}

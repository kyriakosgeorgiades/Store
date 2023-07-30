using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Entities;
using Store.Interface;
using Store.Models.Books.Request;
using Store.Models.Books.Response;

namespace Store.Controllers.Books;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BooksController : Controller
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<BooksController> _logger;
    public BooksController(IBookRepository bookRepository, IMapper mapper, ILogger<BooksController> logger)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        _logger = logger ?? throw new ArgumentNullException(nameof(_logger));
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<BookSearchResponse>))]
    public async Task<IActionResult> GetBooks([FromQuery] string? searchTerm)
    {
        try
        {
            _logger.LogInformation($"Attempting to fetch books with search term: {searchTerm ?? "All"}");

            var books = await _bookRepository.GetAllBooks(searchTerm);

            if (!books.Any())
            {
                _logger.LogInformation($"No books found for the search term: {searchTerm}.");
                return NotFound($"No books found for the search term: {searchTerm}.");
            }

            var booksResponse = _mapper.Map<IEnumerable<BookSearchResponse>>(books);
            _logger.LogInformation($"{booksResponse.Count()} books retrieved successfully.");

            return Ok(booksResponse);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error while fetching books: {ex.Message}");
            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }



    [HttpPost("Book")]
    public async Task<IActionResult> SaveBook(SaveBookRequest book)
    {
        if (ModelState.IsValid)
        {
            var entity = _mapper.Map<Book>(book);
            entity.BookUrl = "https://images.unsplash.com/photo-1495446815901-a7297e633e8d?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWgel";
    
        if (await _bookRepository.DoesISBNExist(entity.ISBN, book.BookId))
            {
                _logger.LogWarning($"Attempt to create/update book with duplicate ISBN {entity.ISBN}.");
                return BadRequest("Book with this ISBN already exists.");
            }

            try
            {
                if (book.BookId == null)
                {
                    _logger.LogInformation($"Attempting to create a new book: {book.BookName}");
                    var createdBook = await _bookRepository.AddBook(entity);
                    _logger.LogInformation($"Book {createdBook.BookId} - {book.BookName} successfully created.");
                    return CreatedAtAction(nameof(SaveBook), new { id = createdBook.BookId }, "Book successfully created.");
                }
                else
                {
                    _logger.LogInformation($"Attempting to update book: {book.BookId}");
                    await _bookRepository.UpdateBook(entity);
                    _logger.LogInformation($"Book {book.BookId} - {book.BookName} successfully updated.");
                    return Ok("Book successfully updated.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error while processing book {book.BookName}. Error: {ex.Message}");
                return StatusCode(500, "Internal server error. Please try again later.");
            }
        }
        _logger.LogWarning("Invalid book data provided.");
        return BadRequest("Invalid book data provided.");
    }

    [HttpGet("Book/{bookId:guid}")]
    public async Task<IActionResult> GetBookById(Guid bookId)
    {
        if (bookId == Guid.Empty)
        {
            _logger.LogWarning("BookId provided is empty.");
            return BadRequest("BookId must not be empty.");
        }

        try
        {
            var book = await _bookRepository.GetBookById(bookId);
            if (book == null)
            {
                _logger.LogWarning($"Book with BookId: {bookId} not found.");
                return NotFound($"No book found with BookId: {bookId}.");
            }

            return Ok(book);
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error fetching book with BookId: {bookId}. Error: {ex.Message}");
            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }

    [HttpDelete("Book/{bookId}")]
    public async Task<IActionResult> DeleteBook(Guid bookId)
    {
        try
        {
            var book = await _bookRepository.GetBookById(bookId);
            if (book == null)
            {
                _logger.LogWarning($"Book with ID {bookId} not found.");
                return NotFound($"Book with ID {bookId} not found.");
            }

            await _bookRepository.DeleteBook(bookId);
            _logger.LogInformation($"Book with ID {bookId} was deleted.");

            return Ok($"Book with ID {bookId} was deleted.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"An error occurred while trying to delete the book with ID {bookId}.");
            return StatusCode(500, "Internal server error. Please try again later.");
        }
    }


}

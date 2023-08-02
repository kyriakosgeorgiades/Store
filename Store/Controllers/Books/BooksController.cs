using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Entities;
using Store.Examples.Books.Request;
using Store.Examples.Books.Response;
using Store.Interface;
using Store.Models.Books.Request;
using Store.Models.Books.Response;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Store.Controllers.Books;

//[Authorize]
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

    /// <summary>
    /// Retrieves all books.
    /// </summary>
    /// <returns>A list of books based on the search query</returns>
    [SwaggerResponseExample(200, typeof(ExampleListOfBookSearchResponse))]
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


    /// <summary>
    /// Saves a book. If a BookId is provided, it updates the book, otherwise, it creates a new book.
    /// </summary>
    /// <param name="book">The request containing details of the book to be saved. Adding a BookId it updates while passing null it creates a new one</param>
    /// <returns>A response indicating the result of the save operation.</returns>
    /// <response code="201">If the book is successfully created.</response>
    /// <response code="200">If the book is successfully updated.</response>
    /// <response code="400">If there's a validation issue or duplicate ISBN.</response>
    /// <response code="500">If there's an internal server error.</response>
    [HttpPost("Book")]
    [SwaggerRequestExample(typeof(SaveBookRequest), typeof(SaveBookRequestExample))]
    [SwaggerResponse(201, "Book successfully created.", Type = typeof(string))]
    [SwaggerResponse(200, "Book successfully updated.", Type = typeof(string))]
    [SwaggerResponse(400, "Bad Request. Due to validation issue or duplicate ISBN.", Type = typeof(string))]
    [SwaggerResponse(500, "Internal server error. Please try again later.", Type = typeof(string))]
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

    /// <summary>
    /// Retrieves a book by its unique identifier.
    /// </summary>
    /// <param name="bookId">The unique identifier of the book.</param>
    /// <returns>The detailed information of the book if found; otherwise, an error message.</returns>
    [HttpGet("Book/{bookId:guid}")]
    [SwaggerOperation(Summary = "Retrieve a book by its ID")]
    [SwaggerResponse(200, "Returns the book with the specified ID.", typeof(Book))] // Assuming the repository returns a `Book` type.
    [SwaggerResponse(400, "If the provided bookId is empty or invalid.", Type = typeof(string))]
    [SwaggerResponse(404, "If no book is found with the provided ID.", Type = typeof(string))]
    [SwaggerResponse(500, "Internal server error. Please try again later.", Type = typeof(string))]
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

    /// <summary>
    /// Deletes a book by its unique identifier.
    /// </summary>
    /// <param name="bookId">The unique identifier of the book to be deleted.</param>
    /// <returns>A success message if the book was deleted; otherwise, an error message.</returns>
    [HttpDelete("Book/{bookId}")]
    [SwaggerOperation(Summary = "Deletes a book by its ID")]
    [SwaggerResponse(200, "The book was successfully deleted.", Type = typeof(string))]
    [SwaggerResponse(404, "If no book is found with the provided ID.", Type = typeof(string))]
    [SwaggerResponse(500, "Internal server error. Please try again later.", Type = typeof(string))]
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

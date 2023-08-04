using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Domains.Interfaces;
using Store.Domains.Interfaces.Books;
using Store.Entities;
using Store.Examples.Books.Response;
using Store.Exceptions;
using Store.Models.Books.Request;
using Store.Models.Books.Response;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Store.Controllers.Books
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : Controller
    {
        private readonly IBookDomainService _bookDomainService;
        private readonly IAuthorDomainService _authorDomainService;
        private readonly ILogger<BooksController> _logger;
        private readonly IMapper _mapper;

        public BooksController(IBookDomainService bookDomainService, ILogger<BooksController> logger, IAuthorDomainService authorDomainService, IMapper mapper)
        {
            _bookDomainService = bookDomainService ?? throw new ArgumentNullException(nameof(bookDomainService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _authorDomainService = authorDomainService ?? throw new ArgumentNullException(nameof(authorDomainService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        /// <summary>
        /// Fetch books based on a search term or fetches all if no term is provided.
        /// </summary>
        /// <param name="searchTerm">Optional search term to filter books</param>
        /// <returns>A list of book search responses</returns>
        [SwaggerResponseExample(200, typeof(ExampleListOfBookSearchResponse))]
        [AllowAnonymous]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<BookSearchResponse>))]
        public async Task<IActionResult> GetBooks([FromQuery] string? searchTerm)
        {
            _logger.LogInformation($"Attempting to fetch books with search term: {searchTerm ?? "All"}");

            var booksResponse = await _bookDomainService.GetAllBooks(searchTerm);
            var wrappedBooksResponse = new BooksWrapperResponse
            {
                Books = booksResponse
            };
            return Ok(wrappedBooksResponse);
        }

        /// <summary>
        /// Save or update a book based on its presence in the system.
        /// </summary>
        /// <param name="book">Details of the book to be saved or updated</param>
        /// <returns>
        /// Returns 201 Created if the book is created, 
        /// 200 OK if the book is updated, 
        /// 400 Bad Request for validation issues or duplicate ISBN, 
        /// or 500 Internal Server Error for unexpected server issues.
        /// </returns>
        [HttpPost("Book")]
        [SwaggerResponse(201, "Book successfully created.", Type = typeof(string))]
        [SwaggerResponse(200, "Book successfully updated.", Type = typeof(string))]
        [SwaggerResponse(400, "Bad Request. Due to validation issue or duplicate ISBN.", Type = typeof(string))]
        [SwaggerResponse(500, "Internal server error. Please try again later.", Type = typeof(string))]
        public async Task<IActionResult> SaveBook(SaveBookRequest book)
        {

            var author = await _authorDomainService.DoesAuthorExist(_mapper.Map<Author>(book.Author));
            book.Author.AuthorId = author.AuthorId;
            book.Author.AuthorName = author.AuthorName;
            var savedBook = await _bookDomainService.SaveBook(book);

            if (book.BookId == null)
            {
                _logger.LogInformation($"Book {savedBook.BookId} - {book.BookName} successfully created.");
                return CreatedAtAction(nameof(SaveBook), new { id = savedBook.BookId }, "Book successfully created.");
            }
            else
            {
                _logger.LogInformation($"Book {book.BookId} - {book.BookName} successfully updated.");
                return Ok("Book successfully updated.");
            }
        }


        /// <summary>
        /// Retrieve a book based on its unique ID.
        /// </summary>
        /// <param name="bookId">The unique ID of the book to be retrieved</param>
        /// <returns>Book object if found, else throws NotFoundException.</returns>
        [HttpGet("Book/{bookId:guid}")]
        [SwaggerOperation(Summary = "Retrieve a book by its ID")]
        public async Task<IActionResult> GetBookById(Guid bookId)
        {
            var book = await _bookDomainService.GetBookById(bookId);

            if (book == null)
            {
                _logger.LogWarning($"Book with BookId: {bookId} not found.");
                throw new NotFoundException($"No book found with BookId: {bookId}.");
            }

            return Ok(book);
        }


        /// <summary>
        /// Deletes a book based on its unique ID.
        /// </summary>
        /// <param name="bookId">The unique ID of the book to be deleted</param>
        /// <returns>Message confirming the deletion</returns>
        [HttpDelete("Book/{bookId}")]
        [SwaggerOperation(Summary = "Deletes a book by its ID")]
        public async Task<IActionResult> DeleteBook(Guid bookId)
        {
            await _bookDomainService.DeleteBook(bookId);
            _logger.LogInformation($"Book with ID {bookId} was deleted.");
            return Ok($"Book with ID {bookId} was deleted.");
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Entities;
using Store.Interface;

namespace Store.Controllers.Books;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class BookController : Controller
{
    private readonly IBookRepository _bookRepository;
    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository ?? throw new ArgumentNullException(nameof(bookRepository));
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
    public IActionResult GetBooks()
    {
        var books = _bookRepository.GetAllBooks();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(books);
    }
}

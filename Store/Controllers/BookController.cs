using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Store.Context;
using Store.Entities;
using Store.Interface;
using System.Reflection.Metadata.Ecma335;

namespace Store.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BookController : Controller
{
    private readonly IBookRepository _bookRepository;
    public BookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    [HttpGet]
    [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
    public IActionResult GetBooks()
    {
        var books = _bookRepository.GetAllBooks();
        
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(books);
    }

    [HttpGet("test")]
    public IActionResult TestConnection()
    {
        using var context = new AppDbContext(new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer("Server=127.0.0.1;Database=BooksDb;User Id=sa;Password=p@ssw0rd!;TrustServerCertificate=true;MultipleActiveResultSets=true").Options);

        var bookCount = context.Books.Count();
        return Ok(bookCount);
    }


}

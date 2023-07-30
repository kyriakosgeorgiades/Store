﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Store.Entities;
using Store.Interface;
using Store.Models.Books.Request;

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
    [ProducesResponseType(200, Type = typeof(IEnumerable<Book>))]
    public IActionResult GetBooks()
    {
        var books = _bookRepository.GetAllBooks();

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(books);
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

}
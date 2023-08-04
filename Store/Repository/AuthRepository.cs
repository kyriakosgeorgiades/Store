using Microsoft.EntityFrameworkCore;
using Store.Context;
using Store.Entities;
using Store.IRepository;

namespace Store.Repository;

/// <summary>
/// Repository implementation for handling Author entities.
/// </summary>

public class AuthorRepository : IAuthorRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorRepository"/> class.
    /// </summary>
    /// <param name="context">The application database context.</param>
    public AuthorRepository(AppDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Adds a new Author entity to the database.
    /// </summary>
    /// <param name="author">The Author entity to be added.</param>
    /// <returns>The added Author entity.</returns>

    public async Task<Author> AddAuthor(Author author)
    {
        _context.Authors.Add(author);
        await _context.SaveChangesAsync();
        return author;
    }


    /// <summary>
    /// Checks if an Author with the specified authorId exists in the database.
    /// </summary>
    /// <param name="authorId">The unique identifier of the Author.</param>
    /// <returns>True if the Author exists, otherwise false.</returns>
    public async Task<bool> AuthorExits(Guid authorId)
    {
        return await _context.Authors.AnyAsync(auth=> auth.AuthorId == authorId);
    }
}

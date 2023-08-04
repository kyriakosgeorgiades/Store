using Store.Domains.Interfaces;
using Store.Entities;
using Store.IRepository;

namespace Store.Domains;

/// <summary>
/// Service responsible for domain-specific operations related to authors.
/// </summary>
public class AuthorDomainService : IAuthorDomainService
{
    private readonly IAuthorRepository _authorRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="AuthorDomainService"/> class.
    /// </summary>
    /// <param name="authorRepository">Repository handling author data operations.</param>
    public AuthorDomainService(IAuthorRepository authorRepository)
    {
        _authorRepository = authorRepository;
    }

    /// <summary>
    /// Checks if a specific author exists; if not, adds them.
    /// </summary>
    /// <param name="author">The author to check or add.</param>
    /// <returns>Returns the existing or newly added author.</returns>
    public async Task<Author> DoesAuthorExist(Author author)
    {
        if (await _authorRepository.AuthorExits(author.AuthorId))
            return author;

        return await _authorRepository.AddAuthor(author);
    }
}

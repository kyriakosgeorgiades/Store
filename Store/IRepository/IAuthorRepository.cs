using Store.Entities;

namespace Store.IRepository;

public interface IAuthorRepository
{
    Task<bool> AuthorExits(Guid authorId);

    Task<Author> AddAuthor(Author author);
}

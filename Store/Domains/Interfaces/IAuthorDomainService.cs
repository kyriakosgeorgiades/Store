using Store.Entities;

namespace Store.Domains.Interfaces;

public interface IAuthorDomainService
{
    Task<Author> DoesAuthorExist(Author author);
}

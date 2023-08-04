using AutoMapper;
using Store.Entities;
using Store.Models.Books.Request;
using Store.Models.Books.Response;

namespace Store.Mappings;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<SaveBookRequest, Book>();

        CreateMap<Book, BookSearchResponse>();

        CreateMap<Author, AuthorSearchResponse>();

        CreateMap<AuthorDto, Author>();

        CreateMap<Author, AuthorDto>();
    }
}

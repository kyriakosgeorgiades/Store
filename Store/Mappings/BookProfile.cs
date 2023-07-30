using AutoMapper;
using Store.Entities;
using Store.Models.Books.Request;

namespace Store.Mappings;

public class BookProfile : Profile
{
    public BookProfile()
    {
        CreateMap<SaveBookRequest, Book>();
    }
}

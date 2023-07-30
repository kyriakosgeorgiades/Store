namespace Store.Mappings;

using AutoMapper;
using Store.Entities;
using Store.Models.Users.Request;

public class UserProfile : Profile
{
    public UserProfile()
    {
        CreateMap<RegisterModelRequest, User>();
    }
}


using AutoMapper;
using BLL.DTO;
using DAL.Models;

namespace BLL
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {

            CreateMap<User, UserDTO>()
            .ForMember(dest => dest.Password, opt => opt.Ignore());

            CreateMap<UserDTO, User>()
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<News, NewsDTO>().ReverseMap();
        }
    }
}

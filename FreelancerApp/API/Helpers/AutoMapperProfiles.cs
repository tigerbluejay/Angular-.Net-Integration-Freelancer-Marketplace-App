using AutoMapper;
using API.Entities;
using API.DTOs;
using System.Linq;

namespace API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // AppUser → MemberDTO
            CreateMap<AppUser, MemberDTO>()
                .ForMember(dest => dest.PhotoUrl,
                    opt => opt.MapFrom(src => src.Photo != null ? src.Photo.Url : null))
                .ForMember(dest => dest.Skills,
                    opt => opt.MapFrom(src => src.Skills.Select(s => s.Name)))
                .ForMember(dest => dest.Roles, opt => opt.MapFrom(src =>
                    src.UserRoles.Select(ur => ur.Role.Name)))
                .ForMember(dest => dest.PortfolioItems, opt => opt.MapFrom(src =>
                    src.PortfolioItems));

            // Photo → PhotoDTO
            CreateMap<Photo, PhotoDTO>();
            // PortfolioItem → PortfolioItemDTO
            CreateMap<PortfolioItem, PortfolioItemDTO>();

        }
    }
}
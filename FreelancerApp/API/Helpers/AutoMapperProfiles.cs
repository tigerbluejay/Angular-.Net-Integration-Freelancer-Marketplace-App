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
                    src.PortfolioItems))
                .ForMember(dest => dest.ClientProjects, opt => opt.MapFrom(src => src.ClientProjects))
                .ForMember(dest => dest.FreelancerProjects, opt => opt.MapFrom(src => src.FreelancerProjects));

            // Photo → PhotoDTO
            CreateMap<Photo, PhotoDTO>();
            // PortfolioItem → PortfolioItemDTO
            CreateMap<PortfolioItem, PortfolioItemDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo != null ? src.Photo.Url : string.Empty));
            CreateMap<Skill, string>().ConvertUsing(s => s.Name);
            CreateMap<Project, ProjectDTO>()
                .ForMember(dest => dest.Skills, opt => opt.MapFrom(src => src.Skills))
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo != null ? src.Photo.Url : null));
            CreateMap<MemberUpdateDTO, AppUser>();
            CreateMap<PortfolioItemCreateDTO, PortfolioItem>();
            CreateMap<PortfolioItemUpdateDTO, PortfolioItem>();
            CreateMap<ProjectCreateDTO, Project>()
                .ForMember(dest => dest.Skills, opt => opt.Ignore());
            CreateMap<ProjectUpdateDTO, Project>()
                .ForMember(dest => dest.Skills, opt => opt.Ignore());
            CreateMap<AppUser, UserDTO>()
                .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo != null ? src.Photo.Url : string.Empty));
            CreateMap<RegisterDTO, AppUser>();
            CreateMap<string, DateOnly>().ConvertUsing(s => DateOnly.Parse(s));
            CreateMap<Project, ProjectBrowseDTO>()
            .ForMember(d => d.SkillNames, opt => opt.MapFrom(s => s.Skills.Select(sk => sk.Name)))
            .ForMember(d => d.ClientUserName, opt => opt.MapFrom(s => s.Client.KnownAs))
            .ForMember(d => d.PhotoUrl, opt => opt.MapFrom(s => s.Photo != null ? s.Photo.Url : null))
            .ForMember(d => d.ClientPhotoUrl, opt => opt.MapFrom(src => src.Client.Photo.Url))
            .ForMember(d => d.ClientUserId, opt => opt.MapFrom(src => src.Client.Id));
            CreateMap<ProposalCreateDTO, Proposal>()
            .ForMember(dest => dest.Created, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.IsAccepted, opt => opt.MapFrom(src => (bool?)null))
            .ForMember(dest => dest.Bid, opt => opt.MapFrom(src => src.Bid))
            .ForMember(dest => dest.Photo, opt => opt.Ignore()); // keep ignoring, attach manually in controller
            CreateMap<Proposal, ProposalDTO>()
            .ForMember(dest => dest.PhotoUrl, opt => opt.MapFrom(src => src.Photo != null ? src.Photo.Url : null))
            .ForMember(dest => dest.FreelancerUsername, opt => opt.MapFrom(src => src.Freelancer != null ? src.Freelancer.UserName : null))
            .ForMember(dest => dest.ClientUsername, opt => opt.MapFrom(src => src.Client != null ? src.Client.UserName : null))
            .ForMember(dest => dest.ProjectTitle, opt => opt.MapFrom(src => src.Project != null ? src.Project.Title : null));
        }
    }
}
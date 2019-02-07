using AdenDemo.Web.Models;
using ALSDE.Dtos;
using AutoMapper;

namespace Aden.Web.Data.Profiles
{
    public class UserProfileProfile : Profile
    {
        public UserProfileProfile()
        {
            CreateMap<AuthenticatedUserDto, UserProfile>()
                .ForMember(d => d.IdentityGuid, opt => opt.MapFrom(s => s.IdentityGuid))
                .ForMember(d => d.EmailAddress, opt => opt.MapFrom(s => s.EmailAddress))
                .ForMember(d => d.FirstName, opt => opt.MapFrom(s => s.Firstname))
                .ForMember(d => d.LastName, opt => opt.MapFrom(s => s.Lastname))
                .ForMember(d => d.FullName, opt => opt.MapFrom(s => $"{s.Firstname} {s.Lastname}"))
                ;
        }

    }
}

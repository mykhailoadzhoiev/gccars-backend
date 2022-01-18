using AutoMapper;
using GCCars.Application.Dto.Identity;
using GCCars.Domain.Models.Identity;

namespace GCCars.Application.Mapping
{
    public class IdentityMappingProfile : Profile
    {
        public IdentityMappingProfile()
        {
            CreateMap<AppUser, UserInfoDto>()
                .ForMember(d => d.Address, o => o.MapFrom(s => s.UserName))
                .ForMember(d => d.UserId, o => o.MapFrom(s => s.Id))
                ;
        }
    }
}

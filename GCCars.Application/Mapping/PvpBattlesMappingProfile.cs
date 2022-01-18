using AutoMapper;
using GCCars.Application.Dto.PvpBattles;
using GCCars.Domain.Models.Battles;
using System.Linq;

namespace GCCars.Application.Mapping
{
    public class PvpBattlesMappingProfile : Profile
    {
        public PvpBattlesMappingProfile()
        {
            int? userId = null;
            CreateMap<PvpBattle, PvpBattleDto>()
                .ForMember(d => d.FightersCount, o => o.MapFrom(s => s.Fighters.Count))
                .ForMember(d => d.OwnerAddress, o => o.MapFrom(s => s.Owner.UserName))
                .ForMember(d => d.OwnerName, o => o.MapFrom(s => "Player" + s.OwnerId.ToString()))
                .ForMember(d => d.IsParticipate, o => o.MapFrom(s => s.Fighters.Any(f => f.Car.OwnerId == userId)))
                ;
        }
    }
}

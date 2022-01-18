using AutoMapper;
using GCCars.Application.Dto.PveBattles;
using GCCars.Domain.Models.Battles;

namespace GCCars.Application.Mapping
{
    public class PveBattlesMappingProfile : Profile
    {
        public PveBattlesMappingProfile()
        {
            CreateMap<PveBattle, PveBattleResultDto>();
        }
    }
}

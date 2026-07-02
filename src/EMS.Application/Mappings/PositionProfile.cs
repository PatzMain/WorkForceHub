using AutoMapper;
using EMS.Domain.Entities;
using EMS.Application.DTOs.Position;

namespace EMS.Application.Mappings
{
    public class PositionProfile : Profile
    {
        public PositionProfile()
        {
            CreateMap<Position, PositionDto>()
                .ForMember(dest => dest.TotalEmployees, opt => opt.MapFrom(src => src.Employees != null ? src.Employees.Count : 0));

            CreateMap<PositionCreateDto, Position>(MemberList.Source);
            CreateMap<PositionUpdateDto, Position>(MemberList.Source);
        }
    }
}

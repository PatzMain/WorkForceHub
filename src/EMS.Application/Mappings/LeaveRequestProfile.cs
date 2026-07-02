using AutoMapper;
using EMS.Domain.Entities;
using EMS.Application.DTOs.Leave;

namespace EMS.Application.Mappings
{
    public class LeaveRequestProfile : Profile
    {
        public LeaveRequestProfile()
        {
            CreateMap<LeaveRequest, LeaveRequestDto>()
                .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : null))
                .ForMember(dest => dest.EmployeeNumber, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.EmployeeNumber : null))
                .ForMember(dest => dest.ReviewedByFullName, opt => opt.MapFrom(src => src.ReviewedBy != null ? $"{src.ReviewedBy.FirstName} {src.ReviewedBy.LastName}" : null));

            CreateMap<LeaveRequestCreateDto, LeaveRequest>(MemberList.Source);
            CreateMap<LeaveRequestUpdateDto, LeaveRequest>(MemberList.Source);
        }
    }
}

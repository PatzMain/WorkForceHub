using AutoMapper;
using EMS.Domain.Entities;
using EMS.Application.DTOs.Employee;

namespace EMS.Application.Mappings
{
    public class EmployeeProfile : Profile
    {
        public EmployeeProfile()
        {
            CreateMap<Employee, EmployeeDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.PositionTitle, opt => opt.MapFrom(src => src.Position != null ? src.Position.Title : null))
                .ForMember(dest => dest.BaseSalary, opt => opt.MapFrom(src => src.Position != null ? src.Position.BaseSalary : 0));

            CreateMap<Employee, EmployeeListDto>()
                .ForMember(dest => dest.DepartmentName, opt => opt.MapFrom(src => src.Department != null ? src.Department.Name : null))
                .ForMember(dest => dest.PositionTitle, opt => opt.MapFrom(src => src.Position != null ? src.Position.Title : null));

            CreateMap<EmployeeCreateDto, Employee>(MemberList.Source);
            CreateMap<EmployeeUpdateDto, Employee>(MemberList.Source);
        }
    }
}

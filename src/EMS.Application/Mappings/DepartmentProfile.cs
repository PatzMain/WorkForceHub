using AutoMapper;
using EMS.Domain.Entities;
using EMS.Application.DTOs.Department;

namespace EMS.Application.Mappings
{
    public class DepartmentProfile : Profile
    {
        public DepartmentProfile()
        {
            CreateMap<Department, DepartmentDto>()
                .ForMember(dest => dest.ManagerFullName, opt => opt.MapFrom(src => src.Manager != null ? $"{src.Manager.FirstName} {src.Manager.LastName}" : null))
                .ForMember(dest => dest.TotalEmployees, opt => opt.MapFrom(src => src.Employees != null ? src.Employees.Count : 0));

            CreateMap<DepartmentCreateDto, Department>(MemberList.Source);
            CreateMap<DepartmentUpdateDto, Department>(MemberList.Source);
        }
    }
}

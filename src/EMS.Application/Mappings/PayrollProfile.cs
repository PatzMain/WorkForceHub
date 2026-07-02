using AutoMapper;
using EMS.Domain.Entities;
using EMS.Application.DTOs.Payroll;

namespace EMS.Application.Mappings
{
    public class PayrollProfile : Profile
    {
        public PayrollProfile()
        {
            CreateMap<PayrollRecord, PayrollRecordDto>()
                .ForMember(dest => dest.EmployeeFullName, opt => opt.MapFrom(src => src.Employee != null ? $"{src.Employee.FirstName} {src.Employee.LastName}" : null))
                .ForMember(dest => dest.EmployeeNumber, opt => opt.MapFrom(src => src.Employee != null ? src.Employee.EmployeeNumber : null));

            CreateMap<PayrollRecordCreateDto, PayrollRecord>(MemberList.Source);
            CreateMap<PayrollRecordUpdateDto, PayrollRecord>(MemberList.Source);
        }
    }
}

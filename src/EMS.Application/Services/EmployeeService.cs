using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EMS.Application.Common;
using EMS.Application.DTOs.Employee;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;

namespace EMS.Application.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repo;
        private readonly IMapper _mapper;

        public EmployeeService(IEmployeeRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<EmployeeDto?> GetByIdAsync(int id)
        {
            var emp = await _repo.GetEmployeeWithDetailsAsync(id);
            if (emp == null)
                throw new NotFoundException(nameof(Employee), id);

            return _mapper.Map<EmployeeDto>(emp);
        }

        public async Task<EmployeeDto?> GetByUserIdAsync(string userId)
        {
            var emp = await _repo.GetEmployeeByUserIdAsync(userId);
            if (emp == null)
                return null;

            return _mapper.Map<EmployeeDto>(emp);
        }

        public async Task<PagedResult<EmployeeListDto>> GetPagedAsync(
            string? search, 
            int? departmentId, 
            int? positionId, 
            string? status, 
            string? sortBy, 
            bool isDescending, 
            int pageNumber, 
            int pageSize)
        {
            var items = await _repo.GetEmployeesPagedAsync(search, departmentId, positionId, status, sortBy, isDescending, pageNumber, pageSize);
            var count = await _repo.GetEmployeesCountAsync(search, departmentId, positionId, status);

            var listDtos = _mapper.Map<List<EmployeeListDto>>(items);
            return new PagedResult<EmployeeListDto>(listDtos, count, pageNumber, pageSize);
        }

        public async Task<EmployeeDto> CreateAsync(EmployeeCreateDto dto)
        {
            var emp = _mapper.Map<Employee>(dto);
            await _repo.AddAsync(emp);
            await _repo.SaveChangesAsync();
            return _mapper.Map<EmployeeDto>(emp);
        }

        public async Task UpdateAsync(EmployeeUpdateDto dto)
        {
            var emp = await _repo.GetByIdAsync(dto.Id);
            if (emp == null)
                throw new NotFoundException(nameof(Employee), dto.Id);

            _mapper.Map(dto, emp);
            await _repo.UpdateAsync(emp);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var emp = await _repo.GetByIdAsync(id);
            if (emp == null)
                throw new NotFoundException(nameof(Employee), id);

            await _repo.DeleteAsync(emp);
            await _repo.SaveChangesAsync();
        }
    }
}

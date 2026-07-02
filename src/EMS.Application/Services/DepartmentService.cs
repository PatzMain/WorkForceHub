using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EMS.Application.Common;
using EMS.Application.DTOs.Department;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;

namespace EMS.Application.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;
        private readonly IMapper _mapper;

        public DepartmentService(IDepartmentRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<DepartmentDto?> GetByIdAsync(int id)
        {
            var dept = await _repo.GetDepartmentWithDetailsAsync(id);
            if (dept == null)
                throw new NotFoundException(nameof(Department), id);

            return _mapper.Map<DepartmentDto>(dept);
        }

        public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync()
        {
            var depts = await _repo.GetAllDepartmentsWithDetailsAsync();
            return _mapper.Map<IReadOnlyList<DepartmentDto>>(depts);
        }

        public async Task<DepartmentDto> CreateAsync(DepartmentCreateDto dto)
        {
            var dept = _mapper.Map<Department>(dto);
            await _repo.AddAsync(dept);
            await _repo.SaveChangesAsync();
            return _mapper.Map<DepartmentDto>(dept);
        }

        public async Task UpdateAsync(DepartmentUpdateDto dto)
        {
            var dept = await _repo.GetByIdAsync(dto.Id);
            if (dept == null)
                throw new NotFoundException(nameof(Department), dto.Id);

            _mapper.Map(dto, dept);
            await _repo.UpdateAsync(dept);
            await _repo.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var dept = await _repo.GetDepartmentWithDetailsAsync(id);
            if (dept == null)
                throw new NotFoundException(nameof(Department), id);

            // Business rule validation: can't delete a department that still has employees
            if (dept.Employees != null && dept.Employees.Count > 0)
            {
                throw new System.Exception("Cannot delete a department that still has active employees.");
            }

            await _repo.DeleteAsync(dept);
            await _repo.SaveChangesAsync();
        }
    }
}

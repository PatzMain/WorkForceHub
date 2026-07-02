using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using EMS.Domain.Entities;
using EMS.Domain.Enums;
using EMS.Application.Interfaces;
using EMS.Infrastructure.Data;

namespace EMS.Infrastructure.Repositories
{
    public class EmployeeRepository : Repository<Employee>, IEmployeeRepository
    {
        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<Employee?> GetEmployeeWithDetailsAsync(int id)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Employee?> GetEmployeeByUserIdAsync(string userId)
        {
            return await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .FirstOrDefaultAsync(e => e.ApplicationUserId == userId);
        }

        public async Task<IReadOnlyList<Employee>> GetEmployeesPagedAsync(
            string? search, 
            int? departmentId, 
            int? positionId, 
            string? status, 
            string? sortBy, 
            bool isDescending, 
            int pageNumber, 
            int pageSize)
        {
            var query = _context.Employees
                .Include(e => e.Department)
                .Include(e => e.Position)
                .AsQueryable();

            query = ApplyFilters(query, search, departmentId, positionId, status);
            query = ApplySorting(query, sortBy, isDescending);

            return await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetEmployeesCountAsync(
            string? search, 
            int? departmentId, 
            int? positionId, 
            string? status)
        {
            var query = _context.Employees.AsQueryable();
            query = ApplyFilters(query, search, departmentId, positionId, status);
            return await query.CountAsync();
        }

        private IQueryable<Employee> ApplyFilters(
            IQueryable<Employee> query, 
            string? search, 
            int? departmentId, 
            int? positionId, 
            string? status)
        {
            if (!string.IsNullOrWhiteSpace(search))
            {
                var cleanSearch = search.Trim().ToLower();
                query = query.Where(e => 
                    e.FirstName.ToLower().Contains(cleanSearch) || 
                    e.LastName.ToLower().Contains(cleanSearch) || 
                    e.Email.ToLower().Contains(cleanSearch) || 
                    e.EmployeeNumber.ToLower().Contains(cleanSearch));
            }

            if (departmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == departmentId.Value);
            }

            if (positionId.HasValue)
            {
                query = query.Where(e => e.PositionId == positionId.Value);
            }

            if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<EmploymentStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(e => e.Status == parsedStatus);
            }

            return query;
        }

        private IQueryable<Employee> ApplySorting(IQueryable<Employee> query, string? sortBy, bool isDescending)
        {
            if (string.IsNullOrWhiteSpace(sortBy))
            {
                return query.OrderBy(e => e.LastName);
            }

            switch (sortBy.ToLower())
            {
                case "name":
                    query = isDescending ? query.OrderByDescending(e => e.LastName).ThenByDescending(e => e.FirstName) 
                                         : query.OrderBy(e => e.LastName).ThenBy(e => e.FirstName);
                    break;
                case "employeenumber":
                    query = isDescending ? query.OrderByDescending(e => e.EmployeeNumber) : query.OrderBy(e => e.EmployeeNumber);
                    break;
                case "datejoined":
                    query = isDescending ? query.OrderByDescending(e => e.DateJoined) : query.OrderBy(e => e.DateJoined);
                    break;
                case "department":
                    query = isDescending ? query.OrderByDescending(e => e.Department != null ? e.Department.Name : "") 
                                         : query.OrderBy(e => e.Department != null ? e.Department.Name : "");
                    break;
                case "position":
                    query = isDescending ? query.OrderByDescending(e => e.Position != null ? e.Position.Title : "") 
                                         : query.OrderBy(e => e.Position != null ? e.Position.Title : "");
                    break;
                default:
                    query = query.OrderBy(e => e.LastName);
                    break;
            }

            return query;
        }
    }
}

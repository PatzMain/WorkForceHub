using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.Application.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IRepository<Employee> _employeeRepo;
        private readonly IDepartmentRepository _deptRepo;
        private readonly IRepository<Position> _posRepo;
        private readonly IRepository<LeaveRequest> _leaveRepo;
        private readonly IRepository<PayrollRecord> _payrollRepo;

        public DashboardService(
            IRepository<Employee> employeeRepo,
            IDepartmentRepository deptRepo,
            IRepository<Position> posRepo,
            IRepository<LeaveRequest> leaveRepo,
            IRepository<PayrollRecord> payrollRepo)
        {
            _employeeRepo = employeeRepo;
            _deptRepo = deptRepo;
            _posRepo = posRepo;
            _leaveRepo = leaveRepo;
            _payrollRepo = payrollRepo;
        }

        public async Task<int> GetTotalEmployeesCountAsync()
        {
            var list = await _employeeRepo.GetAllAsync();
            return list.Count;
        }

        public async Task<int> GetTotalDepartmentsCountAsync()
        {
            var list = await _deptRepo.GetAllAsync();
            return list.Count;
        }

        public async Task<int> GetTotalPositionsCountAsync()
        {
            var list = await _posRepo.GetAllAsync();
            return list.Count;
        }

        public async Task<Dictionary<string, int>> GetEmployeeCountsByDepartmentAsync()
        {
            var depts = await _deptRepo.GetAllDepartmentsWithDetailsAsync();
            return depts.ToDictionary(
                d => d.Name,
                d => d.Employees?.Count ?? 0
            );
        }

        public async Task<Dictionary<string, int>> GetEmployeeCountsByStatusAsync()
        {
            var employees = await _employeeRepo.GetAllAsync();
            return employees
                .GroupBy(e => e.Status)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Count()
                );
        }

        public async Task<Dictionary<string, int>> GetLeaveRequestCountsByStatusAsync()
        {
            var leaves = await _leaveRepo.GetAllAsync();
            return leaves
                .GroupBy(l => l.Status)
                .ToDictionary(
                    g => g.Key.ToString(),
                    g => g.Count()
                );
        }

        public async Task<decimal> GetTotalGrossPayrollThisMonthAsync()
        {
            var records = await _payrollRepo.GetAllAsync();
            var startOfMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            
            return records
                .Where(p => p.PayPeriodStart >= startOfMonth)
                .Sum(p => p.GrossPay);
        }
    }
}

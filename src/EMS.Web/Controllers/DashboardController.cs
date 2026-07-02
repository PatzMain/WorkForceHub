using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMS.Application.Interfaces;

namespace EMS.Web.Controllers
{
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly IDashboardService _dashboardService;
        private readonly IEmployeeService _employeeService;

        public DashboardController(IDashboardService dashboardService, IEmployeeService employeeService)
        {
            _dashboardService = dashboardService;
            _employeeService = employeeService;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalEmployees = await _dashboardService.GetTotalEmployeesCountAsync();
            ViewBag.TotalDepartments = await _dashboardService.GetTotalDepartmentsCountAsync();
            ViewBag.TotalPositions = await _dashboardService.GetTotalPositionsCountAsync();
            ViewBag.TotalGrossPayroll = await _dashboardService.GetTotalGrossPayrollThisMonthAsync();

            ViewBag.EmployeesByDept = await _dashboardService.GetEmployeeCountsByDepartmentAsync();
            ViewBag.EmployeesByStatus = await _dashboardService.GetEmployeeCountsByStatusAsync();
            ViewBag.LeavesByStatus = await _dashboardService.GetLeaveRequestCountsByStatusAsync();

            var recentEmployees = await _employeeService.GetPagedAsync(null, null, null, null, "DateJoined", true, 1, 5);
            ViewBag.RecentEmployees = recentEmployees.Items;

            return View();
        }
    }
}


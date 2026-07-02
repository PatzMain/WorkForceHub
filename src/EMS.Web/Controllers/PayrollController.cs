using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Application.DTOs.Payroll;
using EMS.Application.Interfaces;
using EMS.Domain.Enums;

namespace EMS.Web.Controllers
{
    [Authorize]
    public class PayrollController : Controller
    {
        private readonly IPayrollService _payrollService;
        private readonly IEmployeeService _empService;

        public PayrollController(IPayrollService payrollService, IEmployeeService empService)
        {
            _payrollService = payrollService;
            _empService = empService;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            if (User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Manager"))
            {
                // Management sees all payroll records
                var allPayroll = await _payrollService.GetPagedAsync(pageNumber, pageSize);
                return View(allPayroll);
            }
            else
            {
                // Employees see only their own payroll records
                var employee = await _empService.GetByUserIdAsync(userId);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = "No employee record found for your user account.";
                    return RedirectToAction("Index", "Dashboard");
                }
                var myPayroll = await _payrollService.GetPagedByEmployeeIdAsync(employee.Id, pageNumber, pageSize);
                return View(myPayroll);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var payroll = await _payrollService.GetByIdAsync(id);
                if (payroll == null) return NotFound();

                // Security check
                if (!User.IsInRole("Admin") && !User.IsInRole("HR") && !User.IsInRole("Manager"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var employee = await _empService.GetByUserIdAsync(userId!);
                    if (employee == null || payroll.EmployeeId != employee.Id)
                    {
                        return Forbid();
                    }
                }

                return View(payroll);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create(int? employeeId)
        {
            var employees = await _empService.GetPagedAsync(null, null, null, null, null, false, 1, 1000);
            ViewBag.Employees = new SelectList(employees.Items, "Id", "FullName", employeeId);

            decimal defaultGross = 0;
            if (employeeId.HasValue)
            {
                var emp = await _empService.GetByIdAsync(employeeId.Value);
                if (emp != null)
                {
                    defaultGross = emp.BaseSalary;
                }
            }

            var dto = new PayrollRecordCreateDto
            {
                EmployeeId = employeeId ?? 0,
                PayPeriodStart = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1),
                PayPeriodEnd = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1),
                GrossPay = defaultGross
            };

            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PayrollRecordCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _payrollService.CreateAsync(dto);
                    TempData["SuccessMessage"] = "Payroll record drafted successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            var employees = await _empService.GetPagedAsync(null, null, null, null, null, false, 1, 1000);
            ViewBag.Employees = new SelectList(employees.Items, "Id", "FullName", dto.EmployeeId);
            return View(dto);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process(int id)
        {
            try
            {
                await _payrollService.ProcessAsync(id);
                TempData["SuccessMessage"] = "Payroll record processed successfully!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Pay(int id)
        {
            try
            {
                await _payrollService.PayAsync(id);
                TempData["SuccessMessage"] = "Payroll marked as Paid!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _payrollService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Payroll record deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

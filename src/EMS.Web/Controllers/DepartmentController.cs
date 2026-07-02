using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Application.DTOs.Department;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;

namespace EMS.Web.Controllers
{
    [Authorize]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _deptService;
        private readonly IRepository<Employee> _employeeRepo;

        public DepartmentController(IDepartmentService deptService, IRepository<Employee> employeeRepo)
        {
            _deptService = deptService;
            _employeeRepo = employeeRepo;
        }

        public async Task<IActionResult> Index()
        {
            var depts = await _deptService.GetAllAsync();
            return View(depts);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var dept = await _deptService.GetByIdAsync(id);
                if (dept == null) return NotFound();
                return View(dept);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create()
        {
            var employees = await _employeeRepo.GetAllAsync();
            ViewBag.Employees = new SelectList(employees, "Id", "FullName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _deptService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Department created successfully!";
                return RedirectToAction(nameof(Index));
            }
            var employees = await _employeeRepo.GetAllAsync();
            ViewBag.Employees = new SelectList(employees, "Id", "FullName", dto.ManagerId);
            return View(dto);
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var dept = await _deptService.GetByIdAsync(id);
                if (dept == null) return NotFound();
                
                var dto = new DepartmentUpdateDto
                {
                    Id = dept.Id,
                    Name = dept.Name,
                    Code = dept.Code,
                    ManagerId = dept.ManagerId
                };

                var employees = await _employeeRepo.GetAllAsync();
                ViewBag.Employees = new SelectList(employees, "Id", "FullName", dept.ManagerId);
                return View(dto);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _deptService.UpdateAsync(dto);
                    TempData["SuccessMessage"] = "Department updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            var employees = await _employeeRepo.GetAllAsync();
            ViewBag.Employees = new SelectList(employees, "Id", "FullName", dto.ManagerId);
            return View(dto);
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var dept = await _deptService.GetByIdAsync(id);
                if (dept == null) return NotFound();
                return View(dept);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _deptService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Department deleted successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
    }
}

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using EMS.Application.DTOs.Employee;
using EMS.Application.Interfaces;
using EMS.Domain.Enums;

namespace EMS.Web.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly IEmployeeService _empService;
        private readonly IDepartmentService _deptService;
        private readonly IPositionService _posService;
        private readonly IFileStorageService _fileStorage;

        public EmployeeController(
            IEmployeeService empService,
            IDepartmentService deptService,
            IPositionService posService,
            IFileStorageService fileStorage)
        {
            _empService = empService;
            _deptService = deptService;
            _posService = posService;
            _fileStorage = fileStorage;
        }

        public async Task<IActionResult> Index(
            string? search, 
            int? departmentId, 
            int? positionId, 
            string? status, 
            string? sortBy, 
            bool isDescending = false, 
            int pageNumber = 1, 
            int pageSize = 10)
        {
            var pagedResult = await _empService.GetPagedAsync(search, departmentId, positionId, status, sortBy, isDescending, pageNumber, pageSize);
            
            ViewBag.Search = search;
            ViewBag.DepartmentId = departmentId;
            ViewBag.PositionId = positionId;
            ViewBag.Status = status;
            ViewBag.SortBy = sortBy;
            ViewBag.IsDescending = isDescending;

            ViewBag.Departments = new SelectList(await _deptService.GetAllAsync(), "Id", "Name", departmentId);
            ViewBag.Positions = new SelectList(await _posService.GetAllAsync(), "Id", "Title", positionId);
            
            return View(pagedResult);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var emp = await _empService.GetByIdAsync(id);
                if (emp == null) return NotFound();
                return View(emp);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Create()
        {
            ViewBag.Departments = new SelectList(await _deptService.GetAllAsync(), "Id", "Name");
            ViewBag.Positions = new SelectList(await _posService.GetAllAsync(), "Id", "Title");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeCreateDto dto, IFormFile? profilePicture)
        {
            if (ModelState.IsValid)
            {
                if (profilePicture != null && profilePicture.Length > 0)
                {
                    using (var stream = profilePicture.OpenReadStream())
                    {
                        var fileUrl = await _fileStorage.SaveFileAsync(stream, profilePicture.FileName, "profiles");
                        dto.ProfilePictureUrl = fileUrl;
                    }
                }

                await _empService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Employee created successfully!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.Departments = new SelectList(await _deptService.GetAllAsync(), "Id", "Name", dto.DepartmentId);
            ViewBag.Positions = new SelectList(await _posService.GetAllAsync(), "Id", "Title", dto.PositionId);
            return View(dto);
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var emp = await _empService.GetByIdAsync(id);
                if (emp == null) return NotFound();

                var dto = new EmployeeUpdateDto
                {
                    Id = emp.Id,
                    FirstName = emp.FirstName,
                    LastName = emp.LastName,
                    Email = emp.Email,
                    Phone = emp.Phone,
                    DateOfBirth = emp.DateOfBirth,
                    Gender = emp.Gender,
                    Address = emp.Address,
                    EmployeeNumber = emp.EmployeeNumber,
                    DateJoined = emp.DateJoined,
                    DateTerminated = emp.DateTerminated,
                    Status = emp.Status,
                    ProfilePictureUrl = emp.ProfilePictureUrl,
                    DepartmentId = emp.DepartmentId,
                    PositionId = emp.PositionId,
                    ApplicationUserId = emp.ApplicationUserId
                };

                ViewBag.Departments = new SelectList(await _deptService.GetAllAsync(), "Id", "Name", emp.DepartmentId);
                ViewBag.Positions = new SelectList(await _posService.GetAllAsync(), "Id", "Title", emp.PositionId);
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
        public async Task<IActionResult> Edit(EmployeeUpdateDto dto, IFormFile? profilePicture)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (profilePicture != null && profilePicture.Length > 0)
                    {
                        // Delete old profile picture if exists
                        if (!string.IsNullOrEmpty(dto.ProfilePictureUrl))
                        {
                            await _fileStorage.DeleteFileAsync(dto.ProfilePictureUrl);
                        }

                        using (var stream = profilePicture.OpenReadStream())
                        {
                            var fileUrl = await _fileStorage.SaveFileAsync(stream, profilePicture.FileName, "profiles");
                            dto.ProfilePictureUrl = fileUrl;
                        }
                    }

                    await _empService.UpdateAsync(dto);
                    TempData["SuccessMessage"] = "Employee updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }

            ViewBag.Departments = new SelectList(await _deptService.GetAllAsync(), "Id", "Name", dto.DepartmentId);
            ViewBag.Positions = new SelectList(await _posService.GetAllAsync(), "Id", "Title", dto.PositionId);
            return View(dto);
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var emp = await _empService.GetByIdAsync(id);
                if (emp == null) return NotFound();
                return View(emp);
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
                // Retrieve employee to delete old picture from storage first
                var emp = await _empService.GetByIdAsync(id);
                if (emp != null && !string.IsNullOrEmpty(emp.ProfilePictureUrl))
                {
                    await _fileStorage.DeleteFileAsync(emp.ProfilePictureUrl);
                }

                await _empService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Employee record deleted successfully!";
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

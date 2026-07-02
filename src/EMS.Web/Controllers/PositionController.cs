using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EMS.Application.DTOs.Position;
using EMS.Application.Interfaces;

namespace EMS.Web.Controllers
{
    [Authorize]
    public class PositionController : Controller
    {
        private readonly IPositionService _posService;

        public PositionController(IPositionService posService)
        {
            _posService = posService;
        }

        public async Task<IActionResult> Index()
        {
            var positions = await _posService.GetAllAsync();
            return View(positions);
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var pos = await _posService.GetByIdAsync(id);
                if (pos == null) return NotFound();
                return View(pos);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin,HR")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PositionCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                await _posService.CreateAsync(dto);
                TempData["SuccessMessage"] = "Position created successfully!";
                return RedirectToAction(nameof(Index));
            }
            return View(dto);
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var pos = await _posService.GetByIdAsync(id);
                if (pos == null) return NotFound();

                var dto = new PositionUpdateDto
                {
                    Id = pos.Id,
                    Title = pos.Title,
                    Description = pos.Description,
                    BaseSalary = pos.BaseSalary,
                    SalaryGrade = pos.SalaryGrade
                };
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
        public async Task<IActionResult> Edit(PositionUpdateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _posService.UpdateAsync(dto);
                    TempData["SuccessMessage"] = "Position updated successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(dto);
        }

        [Authorize(Roles = "Admin,HR")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var pos = await _posService.GetByIdAsync(id);
                if (pos == null) return NotFound();
                return View(pos);
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
                await _posService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Position deleted successfully!";
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

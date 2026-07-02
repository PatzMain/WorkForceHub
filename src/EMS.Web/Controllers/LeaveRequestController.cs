using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EMS.Application.DTOs.Leave;
using EMS.Application.Interfaces;
using EMS.Domain.Enums;
using EMS.Infrastructure.Identity;

namespace EMS.Web.Controllers
{
    [Authorize]
    public class LeaveRequestController : Controller
    {
        private readonly ILeaveRequestService _leaveService;
        private readonly IEmployeeService _empService;
        private readonly UserManager<ApplicationUser> _userManager;

        public LeaveRequestController(
            ILeaveRequestService leaveService,
            IEmployeeService empService,
            UserManager<ApplicationUser> userManager)
        {
            _leaveService = leaveService;
            _empService = empService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Challenge();

            if (User.IsInRole("Admin") || User.IsInRole("HR") || User.IsInRole("Manager"))
            {
                // Managers, Admin, and HR view all requests
                var allLeaves = await _leaveService.GetPagedAsync(pageNumber, pageSize);
                return View(allLeaves);
            }
            else
            {
                // Regular employees view only their own requests
                var employee = await _empService.GetByUserIdAsync(userId);
                if (employee == null)
                {
                    TempData["ErrorMessage"] = "No employee record found for your user account.";
                    return RedirectToAction("Index", "Dashboard");
                }
                var myLeaves = await _leaveService.GetPagedByEmployeeIdAsync(employee.Id, pageNumber, pageSize);
                return View(myLeaves);
            }
        }

        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var leave = await _leaveService.GetByIdAsync(id);
                if (leave == null) return NotFound();
                
                // Security check: Employee can only view their own request
                if (!User.IsInRole("Admin") && !User.IsInRole("HR") && !User.IsInRole("Manager"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var employee = await _empService.GetByUserIdAsync(userId!);
                    if (employee == null || leave.EmployeeId != employee.Id)
                    {
                        return Forbid();
                    }
                }

                return View(leave);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Create()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var employee = await _empService.GetByUserIdAsync(userId!);
            if (employee == null)
            {
                TempData["ErrorMessage"] = "You must have an employee record to request leaves.";
                return RedirectToAction(nameof(Index));
            }

            var dto = new LeaveRequestCreateDto
            {
                EmployeeId = employee.Id,
                StartDate = DateTime.Today,
                EndDate = DateTime.Today
            };

            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(LeaveRequestCreateDto dto)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _leaveService.CreateAsync(dto);
                    TempData["SuccessMessage"] = "Leave request submitted successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError(string.Empty, ex.Message);
                }
            }
            return View(dto);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            try
            {
                // Security check
                var leave = await _leaveService.GetByIdAsync(id);
                if (leave == null) return NotFound();

                if (!User.IsInRole("Admin") && !User.IsInRole("HR") && !User.IsInRole("Manager"))
                {
                    var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var employee = await _empService.GetByUserIdAsync(userId!);
                    if (employee == null || leave.EmployeeId != employee.Id)
                    {
                        return Forbid();
                    }
                }

                await _leaveService.CancelAsync(id);
                TempData["SuccessMessage"] = "Leave request cancelled.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id, string? reviewNotes)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var reviewer = await _empService.GetByUserIdAsync(userId!);
                if (reviewer == null)
                {
                    TempData["ErrorMessage"] = "Reviewer employee record not found.";
                    return RedirectToAction(nameof(Index));
                }

                var dto = new LeaveApprovalDto
                {
                    Id = id,
                    Status = LeaveStatus.Approved,
                    ReviewedById = reviewer.Id,
                    ReviewNotes = reviewNotes
                };

                await _leaveService.ApproveOrRejectAsync(dto);
                TempData["SuccessMessage"] = "Leave request approved!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [Authorize(Roles = "Admin,HR,Manager")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Reject(int id, string? reviewNotes)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var reviewer = await _empService.GetByUserIdAsync(userId!);
                if (reviewer == null)
                {
                    TempData["ErrorMessage"] = "Reviewer employee record not found.";
                    return RedirectToAction(nameof(Index));
                }

                var dto = new LeaveApprovalDto
                {
                    Id = id,
                    Status = LeaveStatus.Rejected,
                    ReviewedById = reviewer.Id,
                    ReviewNotes = reviewNotes
                };

                await _leaveService.ApproveOrRejectAsync(dto);
                TempData["SuccessMessage"] = "Leave request rejected.";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}

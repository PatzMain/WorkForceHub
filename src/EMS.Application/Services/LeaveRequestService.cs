using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using EMS.Application.Common;
using EMS.Application.DTOs.Leave;
using EMS.Application.Interfaces;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.Application.Services
{
    public class LeaveRequestService : ILeaveRequestService
    {
        private readonly ILeaveRequestRepository _repo;
        private readonly IMapper _mapper;

        public LeaveRequestService(ILeaveRequestRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }

        public async Task<LeaveRequestDto?> GetByIdAsync(int id)
        {
            var req = await _repo.GetLeaveRequestWithDetailsAsync(id);
            if (req == null)
                throw new NotFoundException(nameof(LeaveRequest), id);

            return _mapper.Map<LeaveRequestDto>(req);
        }

        public async Task<IReadOnlyList<LeaveRequestDto>> GetByEmployeeIdAsync(int employeeId)
        {
            var list = await _repo.GetLeaveRequestsByEmployeeIdAsync(employeeId);
            return _mapper.Map<IReadOnlyList<LeaveRequestDto>>(list);
        }

        public async Task<IReadOnlyList<LeaveRequestDto>> GetAllAsync()
        {
            var list = await _repo.GetAllLeaveRequestsWithDetailsAsync();
            return _mapper.Map<IReadOnlyList<LeaveRequestDto>>(list);
        }

        public async Task<PagedResult<LeaveRequestDto>> GetPagedAsync(int pageNumber, int pageSize)
        {
            var list = await _repo.GetAllLeaveRequestsWithDetailsAsync();
            var dtos = _mapper.Map<List<LeaveRequestDto>>(list);
            
            var pagedItems = System.Linq.Enumerable.ToList(
                System.Linq.Enumerable.Take(
                    System.Linq.Enumerable.Skip(dtos, (pageNumber - 1) * pageSize),
                    pageSize
                )
            );
            
            return new PagedResult<LeaveRequestDto>(pagedItems, dtos.Count, pageNumber, pageSize);
        }

        public async Task<PagedResult<LeaveRequestDto>> GetPagedByEmployeeIdAsync(int employeeId, int pageNumber, int pageSize)
        {
            var list = await _repo.GetLeaveRequestsByEmployeeIdAsync(employeeId);
            var dtos = _mapper.Map<List<LeaveRequestDto>>(list);
            
            var pagedItems = System.Linq.Enumerable.ToList(
                System.Linq.Enumerable.Take(
                    System.Linq.Enumerable.Skip(dtos, (pageNumber - 1) * pageSize),
                    pageSize
                )
            );
            
            return new PagedResult<LeaveRequestDto>(pagedItems, dtos.Count, pageNumber, pageSize);
        }

        public async Task<LeaveRequestDto> CreateAsync(LeaveRequestCreateDto dto)
        {
            // Business rule: leave request dates cannot overlap an approved request
            var isOverlapping = await _repo.CheckOverlappingApprovedRequestsAsync(dto.EmployeeId, dto.StartDate, dto.EndDate);
            if (isOverlapping)
            {
                throw new Exception("Leave request dates overlap an already approved leave request.");
            }

            var req = _mapper.Map<LeaveRequest>(dto);
            req.Status = LeaveStatus.Pending;
            
            await _repo.AddAsync(req);
            await _repo.SaveChangesAsync();
            
            return _mapper.Map<LeaveRequestDto>(req);
        }

        public async Task UpdateAsync(LeaveRequestUpdateDto dto)
        {
            var req = await _repo.GetByIdAsync(dto.Id);
            if (req == null)
                throw new NotFoundException(nameof(LeaveRequest), dto.Id);

            if (req.Status != LeaveStatus.Pending)
            {
                throw new Exception("Only pending leave requests can be updated.");
            }

            // Check overlapping
            var isOverlapping = await _repo.CheckOverlappingApprovedRequestsAsync(req.EmployeeId, dto.StartDate, dto.EndDate);
            if (isOverlapping)
            {
                throw new Exception("Leave request dates overlap an already approved leave request.");
            }

            _mapper.Map(dto, req);
            await _repo.UpdateAsync(req);
            await _repo.SaveChangesAsync();
        }

        public async Task CancelAsync(int id)
        {
            var req = await _repo.GetByIdAsync(id);
            if (req == null)
                throw new NotFoundException(nameof(LeaveRequest), id);

            if (req.Status != LeaveStatus.Pending)
            {
                throw new Exception("Only pending leave requests can be cancelled.");
            }

            req.Status = LeaveStatus.Cancelled;
            await _repo.UpdateAsync(req);
            await _repo.SaveChangesAsync();
        }

        public async Task ApproveOrRejectAsync(LeaveApprovalDto dto)
        {
            var req = await _repo.GetByIdAsync(dto.Id);
            if (req == null)
                throw new NotFoundException(nameof(LeaveRequest), dto.Id);

            if (req.Status != LeaveStatus.Pending)
            {
                throw new Exception("Only pending leave requests can be approved or rejected.");
            }

            if (dto.Status != LeaveStatus.Approved && dto.Status != LeaveStatus.Rejected)
            {
                throw new Exception("Invalid status transition for leave approval/rejection.");
            }

            req.Status = dto.Status;
            req.ReviewedById = dto.ReviewedById;
            req.ReviewNotes = dto.ReviewNotes;
            req.DateReviewed = DateTime.UtcNow;

            await _repo.UpdateAsync(req);
            await _repo.SaveChangesAsync();
        }
    }
}

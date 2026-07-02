using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using EMS.Application.Common;
using EMS.Application.DTOs.Leave;
using EMS.Application.Interfaces;
using EMS.Application.Mappings;
using EMS.Application.Services;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.UnitTests.Services
{
    public class LeaveRequestServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<ILeaveRequestRepository> _mockRepo;
        private readonly LeaveRequestService _service;

        public LeaveRequestServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<LeaveRequestProfile>());
            _mapper = config.CreateMapper();
            _mockRepo = new Mock<ILeaveRequestRepository>();
            _service = new LeaveRequestService(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_WithOverlappingApprovedRequests_ThrowsException()
        {
            // Arrange
            var dto = new LeaveRequestCreateDto 
            { 
                EmployeeId = 1, 
                StartDate = DateTime.Today, 
                EndDate = DateTime.Today.AddDays(2) 
            };
            
            _mockRepo.Setup(r => r.CheckOverlappingApprovedRequestsAsync(1, dto.StartDate, dto.EndDate))
                .ReturnsAsync(true); // Mocking that an approved overlap exists

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CreateAsync(dto));
            Assert.Equal("Leave request dates overlap an already approved leave request.", ex.Message);
        }

        [Fact]
        public async Task CancelAsync_WithNonPendingStatus_ThrowsException()
        {
            // Arrange
            var leave = new LeaveRequest { Id = 1, Status = LeaveStatus.Approved };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(leave);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<Exception>(() => _service.CancelAsync(1));
            Assert.Equal("Only pending leave requests can be cancelled.", ex.Message);
        }

        [Fact]
        public async Task CancelAsync_WithPendingStatus_UpdatesStatusToCancelled()
        {
            // Arrange
            var leave = new LeaveRequest { Id = 1, Status = LeaveStatus.Pending };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(leave);

            // Act
            await _service.CancelAsync(1);

            // Assert
            Assert.Equal(LeaveStatus.Cancelled, leave.Status);
            _mockRepo.Verify(r => r.UpdateAsync(leave), Times.Once);
        }
    }
}

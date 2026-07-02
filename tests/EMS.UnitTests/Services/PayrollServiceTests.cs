using System;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using Xunit;
using EMS.Application.Common;
using EMS.Application.DTOs.Payroll;
using EMS.Application.Interfaces;
using EMS.Application.Mappings;
using EMS.Application.Services;
using EMS.Domain.Entities;
using EMS.Domain.Enums;

namespace EMS.UnitTests.Services
{
    public class PayrollServiceTests
    {
        private readonly IMapper _mapper;
        private readonly Mock<IPayrollRepository> _mockRepo;
        private readonly PayrollService _service;

        public PayrollServiceTests()
        {
            var config = new MapperConfiguration(cfg => cfg.AddProfile<PayrollProfile>());
            _mapper = config.CreateMapper();
            _mockRepo = new Mock<IPayrollRepository>();
            _service = new PayrollService(_mockRepo.Object, _mapper);
        }

        [Fact]
        public async Task CreateAsync_CalculatesNetPayAndSetsStatusDraft()
        {
            // Arrange
            var dto = new PayrollRecordCreateDto
            {
                EmployeeId = 1,
                GrossPay = 5000,
                Deductions = 1000,
                PayPeriodStart = DateTime.Today,
                PayPeriodEnd = DateTime.Today.AddMonths(1)
            };

            // Act
            var result = await _service.CreateAsync(dto);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(5000, result.GrossPay);
            Assert.Equal(1000, result.Deductions);
            Assert.Equal(4000, result.NetPay); // Gross - Deductions
            Assert.Equal(PayrollStatus.Draft, result.Status);
        }

        [Fact]
        public async Task ProcessAsync_FromDraft_SetsStatusProcessed()
        {
            // Arrange
            var record = new PayrollRecord { Id = 1, Status = PayrollStatus.Draft };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(record);

            // Act
            await _service.ProcessAsync(1);

            // Assert
            Assert.Equal(PayrollStatus.Processed, record.Status);
            _mockRepo.Verify(r => r.UpdateAsync(record), Times.Once);
        }

        [Fact]
        public async Task PayAsync_FromProcessed_SetsStatusPaidAndPaymentDate()
        {
            // Arrange
            var record = new PayrollRecord { Id = 1, Status = PayrollStatus.Processed };
            _mockRepo.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(record);

            // Act
            await _service.PayAsync(1);

            // Assert
            Assert.Equal(PayrollStatus.Paid, record.Status);
            Assert.NotNull(record.PaymentDate);
            _mockRepo.Verify(r => r.UpdateAsync(record), Times.Once);
        }
    }
}

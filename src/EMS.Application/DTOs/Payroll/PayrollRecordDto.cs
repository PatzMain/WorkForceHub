using System;
using EMS.Domain.Enums;

namespace EMS.Application.DTOs.Payroll
{
    public class PayrollRecordDto
    {
        public int Id { get; set; }
        
        public int EmployeeId { get; set; }
        public string? EmployeeFullName { get; set; }
        public string? EmployeeNumber { get; set; }

        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }

        public decimal GrossPay { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }

        public PayrollStatus Status { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}

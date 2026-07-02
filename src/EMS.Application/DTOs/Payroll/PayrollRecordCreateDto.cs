using System;

namespace EMS.Application.DTOs.Payroll
{
    public class PayrollRecordCreateDto
    {
        public int EmployeeId { get; set; }
        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }
        public decimal GrossPay { get; set; }
        public decimal Deductions { get; set; }
        public string? Notes { get; set; }
    }
}

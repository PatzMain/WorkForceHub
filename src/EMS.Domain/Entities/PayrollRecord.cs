using System;
using EMS.Domain.Common;
using EMS.Domain.Enums;

namespace EMS.Domain.Entities
{
    public class PayrollRecord : BaseEntity
    {
        public int EmployeeId { get; set; }
        public Employee? Employee { get; set; }

        public DateTime PayPeriodStart { get; set; }
        public DateTime PayPeriodEnd { get; set; }

        public decimal GrossPay { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetPay { get; set; }

        public PayrollStatus Status { get; set; } = PayrollStatus.Draft;
        public DateTime? PaymentDate { get; set; }
        
        public string? Notes { get; set; }
    }
}

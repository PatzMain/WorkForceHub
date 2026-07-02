namespace EMS.Application.DTOs.Payroll
{
    public class PayrollRecordUpdateDto
    {
        public int Id { get; set; }
        public decimal GrossPay { get; set; }
        public decimal Deductions { get; set; }
        public string? Notes { get; set; }
    }
}

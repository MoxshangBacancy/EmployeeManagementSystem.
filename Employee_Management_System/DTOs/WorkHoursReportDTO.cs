namespace Employee_Management_System.DTOs
{
    public class WorkHoursReportDTO
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; }
        public string TimePeriod { get; set; } 
        public int TotalHoursWorked { get; set; }
    }
}

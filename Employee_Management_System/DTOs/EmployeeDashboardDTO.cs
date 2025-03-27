namespace Employee_Management_System.DTOs
{
    public class EmployeeDashboardDTO
    {
        public int TotalLoggedHours { get; set; }
        public int LeaveBalance { get; set; }
        public List<TimesheetDTO> RecentTimesheets { get; set; }
    }
}

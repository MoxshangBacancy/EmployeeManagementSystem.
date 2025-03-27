namespace Employee_Management_System.DTOs
{
    public class AdminDashboardDTO
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int PendingLeaveRequests { get; set; }
        public Dictionary<string, int> DepartmentWiseEmployeeCount { get; set; }
    }
}

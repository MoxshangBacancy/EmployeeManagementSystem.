namespace Employee_Management_System.Request
{
    public class EmployeeUpdateRequest
    {
        public string? Phone { get; set; }
        public string? TechStack { get; set; }
        public string? Address { get; set; }
        public bool? IsActive { get; set; }
    }
}

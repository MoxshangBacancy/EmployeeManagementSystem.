namespace Employee_Management_System.Data.Entities
{
    public class Employee
    {
        public int EmployeeId { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string? Address { get; set; }
        public int? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string ?TechStack { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public ICollection<Leave> Leaves { get; set; } = new List<Leave>();  
        public ICollection<Timesheet> Timesheets { get; set; } = new List<Timesheet>();  

    }
}

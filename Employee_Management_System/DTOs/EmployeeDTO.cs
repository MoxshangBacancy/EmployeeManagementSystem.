namespace Employee_Management_System.DTOs
{
    public class EmployeeDTO
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Department { get; set; }
        public string TechStack { get; set; }
        public string Address { get; set; }
        public DateTime DateOfBirth { get; set; }
        public List<TimesheetDTO> Timesheets { get; set; } 
    }

}

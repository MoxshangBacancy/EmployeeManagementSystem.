namespace Employee_Management_System.DTOs
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<EmployeeProfileDTO> Employees { get; set; } = new List<EmployeeProfileDTO>();
    }
}

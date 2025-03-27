namespace Employee_Management_System.Data.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    }
}

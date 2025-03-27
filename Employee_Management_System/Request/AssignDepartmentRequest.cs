namespace Employee_Management_System.Request
{
    public class AssignDepartmentRequest
    {
        public int UserId { get; set; }
        public int DepartmentId { get; set; }
        public int? EmployeeId { get; set; }
    }
}

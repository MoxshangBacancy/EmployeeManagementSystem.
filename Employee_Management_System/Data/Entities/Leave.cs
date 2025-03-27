namespace Employee_Management_System.Data.Entities
{
    public class Leave
    {
        public int LeaveId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public DateTime AppliedAt { get; set; }

        public Employee Employee { get; set; }
    }
}

namespace Employee_Management_System.Request
{
    public class LeaveRequestDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string LeaveType { get; set; }
        public string Reason { get; set; }
    }
}

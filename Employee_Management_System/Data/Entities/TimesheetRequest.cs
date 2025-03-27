namespace Employee_Management_System.Data.Entities
{
    public class TimesheetRequest
    {
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string? Description { get; set; }
    }
}


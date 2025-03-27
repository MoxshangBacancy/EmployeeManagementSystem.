namespace Employee_Management_System.Request
{
    public class TimesheetRequest
    {
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Description { get; set; }
    }

}

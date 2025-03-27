namespace Employee_Management_System.DTOs
{
    public class TimesheetDTO
    {
        public int TimesheetId { get; set; }
        public DateTime Date { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public int TotalHours { get; set; }
        public string Description { get; set; }
    }

}

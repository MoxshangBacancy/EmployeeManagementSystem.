namespace Employee_Management_System.Data.Entities
{
    public class Timesheet
    {
        public int TimesheetId { get; set; } 
        public int EmployeeId { get; set; }  
        public DateTime Date { get; set; }  
        public TimeSpan StartTime { get; set; }  
        public TimeSpan EndTime { get; set; } 
        public int TotalHours { get; set; }  
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }  

        public Employee Employee { get; set; }  

    }
}

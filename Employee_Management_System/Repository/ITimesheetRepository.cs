using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Repository
{
    public interface ITimesheetRepository
    {
        Task<IEnumerable<Timesheet>> GetEmployeeTimesheetsAsync(int employeeId);
        Task<Timesheet?> GetTimesheetByIdAsync(int timesheetId);
        Task<bool> AddTimesheetAsync(Timesheet timesheet);
        Task<bool> UpdateTimesheetAsync(Timesheet timesheet);
        Task<Timesheet?> GetTimesheetByDateAsync(int employeeId, DateTime date);

    }
}

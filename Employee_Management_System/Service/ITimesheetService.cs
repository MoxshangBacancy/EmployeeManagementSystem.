using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Service
{
    public interface ITimesheetService
    {
        Task<IEnumerable<Timesheet>> GetEmployeeTimesheetsAsync(int employeeId);
        Task<Timesheet?> GetTimesheetByIdAsync(int timesheetId);
        Task<(bool IsSuccess, string Message)> AddTimesheetAsync(int employeeId, TimesheetRequest request);
        Task<bool> UpdateTimesheetAsync(int timesheetId, TimesheetRequest request);

        Task<int> GetTotalLoggedHoursAsync(int employeeId);
        Task<List<Timesheet>> GetRecentTimesheetsAsync(int employeeId, int count);
        Task<Timesheet?> GetTimesheetByDateAsync(int employeeId, DateTime date);


    }
}

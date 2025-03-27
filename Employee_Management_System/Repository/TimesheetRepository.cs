using Employee_Management_System.Data.Entities;
using Employee_Management_System.Data;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Repository
{
    public class TimesheetRepository : ITimesheetRepository
    {
        private readonly AppDbContext _context;

        public TimesheetRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Timesheet>> GetEmployeeTimesheetsAsync(int employeeId)
        {
            return await _context.Timesheets
                .Where(t => t.EmployeeId == employeeId)
                .OrderByDescending(t => t.Date)
                .ToListAsync();
        }

        public async Task<Timesheet?> GetTimesheetByIdAsync(int timesheetId)
        {
            return await _context.Timesheets.FindAsync(timesheetId);
        }

        public async Task<bool> AddTimesheetAsync(Timesheet timesheet)
        {
            await _context.Timesheets.AddAsync(timesheet);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateTimesheetAsync(Timesheet timesheet)
        {
            var existingTimesheet = await _context.Timesheets.FindAsync(timesheet.TimesheetId);
            if (existingTimesheet == null) return false;

            existingTimesheet.StartTime = timesheet.StartTime;
            existingTimesheet.EndTime = timesheet.EndTime;
            existingTimesheet.TotalHours = timesheet.TotalHours;
            existingTimesheet.Description = timesheet.Description;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Timesheet?> GetTimesheetByDateAsync(int employeeId, DateTime date)
        {
            return await _context.Timesheets
                .FirstOrDefaultAsync(t => t.EmployeeId == employeeId && t.Date == date);
        }
    }
}

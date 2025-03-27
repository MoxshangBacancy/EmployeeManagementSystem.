using System.Globalization;
using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.Repository;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Service
{
    public class TimesheetService : ITimesheetService
    {
        private readonly ITimesheetRepository _timesheetRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AppDbContext _context;

        public TimesheetService(ITimesheetRepository timesheetRepository, IEmployeeRepository employeeRepository, AppDbContext context)
        {
            _timesheetRepository = timesheetRepository;
            _employeeRepository = employeeRepository;
            _context = context;
        }

        public async Task<IEnumerable<Timesheet>> GetEmployeeTimesheetsAsync(int employeeId)
        {
            return await _timesheetRepository.GetEmployeeTimesheetsAsync(employeeId);
        }

        public async Task<Timesheet?> GetTimesheetByIdAsync(int timesheetId)
        {
            return await _timesheetRepository.GetTimesheetByIdAsync(timesheetId);
        }
        public async Task<Timesheet?> GetTimesheetByDateAsync(int employeeId, DateTime date)
        {
            return await _timesheetRepository.GetTimesheetByDateAsync(employeeId, date);
        }

        public async Task<(bool IsSuccess, string Message)> AddTimesheetAsync(int employeeId, TimesheetRequest request)
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
                return (false, "Employee not found.");

            if (request.Date.Date > DateTime.UtcNow.Date)
                return (false, "You cannot log work hours for future dates.");

            var existingTimesheet = await GetTimesheetByDateAsync(employeeId, request.Date.Date);
            if (existingTimesheet != null)
                return (false, "You have already logged work hours for today.");

            TimeSpan startTime = TimeSpan.Parse(request.StartTime.ToString() );
            TimeSpan endTime = TimeSpan.Parse(request.EndTime.ToString());

            if (endTime <= startTime)
                return (false, "End time must be later than start time.");

            var totalHours = (endTime - startTime).TotalHours;

            var timesheet = new Timesheet
            {
                EmployeeId = employeeId,
                Date = request.Date,
                StartTime = startTime,
                EndTime = endTime,
                TotalHours = (int)Math.Round(totalHours),
                Description = request.Description,
                CreatedAt = DateTime.UtcNow
            };

            var isSuccess = await _timesheetRepository.AddTimesheetAsync(timesheet);
            return isSuccess ? (true, "Work hours logged successfully.") : (false, "Failed to log work hours due to an internal error.");
        }

        public async Task<bool> UpdateTimesheetAsync(int timesheetId, TimesheetRequest request)
        {
            var timesheet = await _timesheetRepository.GetTimesheetByIdAsync(timesheetId);
            if (timesheet == null) return false;

            timesheet.StartTime = request.StartTime;
            timesheet.EndTime = request.EndTime;
            timesheet.TotalHours = (request.EndTime - request.StartTime).Hours;
            timesheet.Description = request.Description;

            return await _timesheetRepository.UpdateTimesheetAsync(timesheet);
        }

        public async Task<int> GetTotalLoggedHoursAsync(int employeeId)
        {
            return await _context.Timesheets.Where(t => t.EmployeeId == employeeId)
                                            .SumAsync(t => t.TotalHours);
        }

        public async Task<List<Timesheet>> GetRecentTimesheetsAsync(int employeeId, int count)
        {
            return await _context.Timesheets.Where(t => t.EmployeeId == employeeId)
                                            .OrderByDescending(t => t.Date)
                                            .Take(count)
                                            .ToListAsync();
        }

    }
}

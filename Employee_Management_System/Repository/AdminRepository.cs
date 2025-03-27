using Employee_Management_System.Controllers;
using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs;
using Employee_Management_System.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class AdminRepository : IAdminRepository
{
    private readonly AppDbContext _context;
    private readonly ILogger<AdminReportController> _logger;

    public AdminRepository(AppDbContext context, ILogger<AdminReportController> logger)
    {
        _context = context;
        _logger = logger;

    }
    public async Task<IEnumerable<Employee>> GetAllEmployeesWithTimesheetsAsync()
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Department)
            .Include(e => e.Timesheets)
            .ToListAsync();
    }
    public async Task<Employee?> GetEmployeeByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.User)
            .Include(e => e.Department)
            .Include(e => e.Timesheets)
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }

    public async Task<bool> UpdateEmployeeAsync(Employee employee, bool? isActive = null)
    {
        var existingEmployee = await _context.Employees
            .Include(e => e.User)
            .FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);

        if (existingEmployee == null || existingEmployee.User == null) return false;

        if (!string.IsNullOrEmpty(employee.User.Phone) && employee.User.Phone != existingEmployee.User.Phone)
        {
            var phoneExists = await _context.Users.AnyAsync(u => u.Phone == employee.User.Phone && u.Id != existingEmployee.User.Id);
            if (phoneExists) throw new Exception("Phone number is already in use by another employee.");
            existingEmployee.User.Phone = employee.User.Phone;
        }

        existingEmployee.TechStack = employee.TechStack ?? existingEmployee.TechStack;
        existingEmployee.Address = employee.Address ?? existingEmployee.Address;

        if (isActive.HasValue) existingEmployee.User.IsActive = isActive.Value;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<IEnumerable<Timesheet>> GetEmployeeTimesheetsAsync(int employeeId)
    {
        return await _context.Timesheets
            .Where(t => t.EmployeeId == employeeId)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<Timesheet>> GetAllTimesheetsAsync()
    {
        return await _context.Timesheets
            .AsNoTracking()
            .Include(t => t.Employee)
            .ThenInclude(e => e.User)
            .OrderByDescending(t => t.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkHoursReportDTO>> GetEmployeeWorkHoursReportAsync(string periodType, int year, int monthOrWeek)
    {
        var query = _context.Timesheets
            .Include(t => t.Employee)
            .ThenInclude(e => e.User)
            .Where(t => t.Date.Year == year);

        if (periodType.ToLower() == "monthly")
        {
            query = query.Where(t => t.Date.Month == monthOrWeek);
        }
        else if (periodType.ToLower() == "weekly")
        {
            var firstDayOfYear = new DateTime(year, 1, 1);
            var firstMonday = firstDayOfYear.AddDays((8 - (int)firstDayOfYear.DayOfWeek) % 7);
            var startOfWeek = firstMonday.AddDays((monthOrWeek - 1) * 7).Date;
            var endOfWeek = startOfWeek.AddDays(6).Date.AddHours(23).AddMinutes(59).AddSeconds(59);

            query = query.Where(t => t.Date >= startOfWeek && t.Date <= endOfWeek);
        }

        var groupedData = await query
            .GroupBy(t => new { t.EmployeeId, t.Employee.User.FirstName, t.Employee.User.LastName })
            .Select(g => new WorkHoursReportDTO
            {
                EmployeeId = g.Key.EmployeeId,
                EmployeeName = $"{g.Key.FirstName} {g.Key.LastName}",
                TimePeriod = periodType.ToLower() == "monthly"
                             ? $"{new DateTime(year, monthOrWeek, 1):MMMM yyyy}"
                             : $"Week {monthOrWeek} of {year}",
                TotalHoursWorked = g.Sum(t => t.TotalHours)
            })
            .ToListAsync();

        return groupedData;
    }


    public async Task<bool> DeleteEmployeeAsync(int id)
    {
        var employee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.EmployeeId == id);
        if (employee == null) return false;

        _context.Users.Remove(employee.User); 
        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsPhoneNumberExistsAsync(string phone, int excludeEmployeeId)
    {
        return await _context.Employees
            .AnyAsync(e => e.User.Phone == phone && e.EmployeeId != excludeEmployeeId);
    }
}

using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs;
using Employee_Management_System.Request;

public interface IAdminService
{
    Task<IEnumerable<Employee>> GetAllEmployeesAsync();
    Task<Employee?> GetEmployeeByIdAsync(int id);
    Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateRequest request); 
    Task<IEnumerable<Timesheet>> GetEmployeeTimesheetsAsync(int employeeId);
    Task<IEnumerable<Timesheet>> GetAllTimesheetsAsync();
    Task<IEnumerable<WorkHoursReportDTO>> GetEmployeeWorkHoursReportAsync(string periodType, int year, int month);
    Task<bool> DeleteEmployeeAsync(int id);


}


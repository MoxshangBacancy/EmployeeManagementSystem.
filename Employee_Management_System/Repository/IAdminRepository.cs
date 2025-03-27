using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Repository
{
    using Employee_Management_System.Data.Entities;
    using Employee_Management_System.DTOs;

    public interface IAdminRepository
    {
        Task<IEnumerable<Employee>> GetAllEmployeesWithTimesheetsAsync();
        Task<Employee?> GetEmployeeByIdAsync(int id);
        Task<bool> UpdateEmployeeAsync(Employee employee, bool? isActive = null); 
        Task<IEnumerable<Timesheet>> GetEmployeeTimesheetsAsync(int employeeId);
        Task<IEnumerable<Timesheet>> GetAllTimesheetsAsync();
        Task<IEnumerable<WorkHoursReportDTO>> GetEmployeeWorkHoursReportAsync(string periodType, int year, int month);
        Task<bool> DeleteEmployeeAsync(int id);
        Task<bool> IsPhoneNumberExistsAsync(string phone, int excludeEmployeeId);

    }

}

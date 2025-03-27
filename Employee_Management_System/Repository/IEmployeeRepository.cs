using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Repository
{
    public interface IEmployeeRepository
    {
        Task<Employee?> GetEmployeeByEmailAsync(string email);
        Task<Employee?> GetByIdAsync(int id);
        Task<bool> UpdateEmployeeProfileAsync(Employee employee);
        Task<bool> ResetPasswordAsync(string email, string newPassword);
        Task<Employee> GetEmployeeByIdAsync(int employeeId);

    }
}

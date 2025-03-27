using Employee_Management_System.Data.Entities;

public interface IEmployeeService
{
    Task<Employee?> GetEmployeeProfileAsync(string email);
    Task<bool> UpdateEmployeeProfileAsync(Employee employee);
    Task<bool> ResetPasswordAsync(string email, string newPassword);

    Task<int> GetTotalEmployeesAsync();
    Task<int> GetActiveEmployeesAsync();

}

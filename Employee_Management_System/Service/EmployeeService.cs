using Employee_Management_System.Data.Entities;
using Employee_Management_System.Repository;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Employee_Management_System.Data;

namespace Employee_Management_System.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly AppDbContext _context; 

        public EmployeeService(IEmployeeRepository employeeRepository, AppDbContext context)
        {
            _employeeRepository = employeeRepository;
            _context = context;
        }

        public async Task<Employee?> GetEmployeeProfileAsync(string email)
        {
            return await _context.Employees
                         .Include(e => e.User)  
                         .Include(e => e.Department)  
                         .FirstOrDefaultAsync(e => e.User.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UpdateEmployeeProfileAsync(Employee employee)
        {
            return await _employeeRepository.UpdateEmployeeProfileAsync(employee);
        }

        public async Task<bool> ResetPasswordAsync(string email, string newPassword)
        {
            return await _employeeRepository.ResetPasswordAsync(email, newPassword);
        }

        public async Task<int> GetTotalEmployeesAsync()
        {
            return await _context.Employees.CountAsync();
        }

        public async Task<int> GetActiveEmployeesAsync()
        {
            return await _context.Employees.CountAsync(e => e.User.IsActive);
        }
    }
}

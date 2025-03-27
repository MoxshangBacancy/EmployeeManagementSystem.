using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs; 
using Employee_Management_System.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

public class EmployeeRepository : IEmployeeRepository
{
    private readonly AppDbContext _context;

    public EmployeeRepository(AppDbContext context)
    {
        _context = context;
    }
    public async Task<Employee?> GetEmployeeByEmailAsync(string email)
    {
        return await _context.Employees
            .Include(e => e.User)
            .ThenInclude(u => u.Role) 
            .FirstOrDefaultAsync(e => e.User.Email == email);
    }

    public async Task<Employee?> GetByIdAsync(int id)
    {
        return await _context.Employees
            .Include(e => e.User) 
            .FirstOrDefaultAsync(e => e.EmployeeId == id);
    }

    public async Task<bool> UpdateEmployeeProfileAsync(Employee employee)
    {
        var existingEmployee = await _context.Employees.Include(e => e.User).FirstOrDefaultAsync(e => e.EmployeeId == employee.EmployeeId);
        if (existingEmployee == null) return false;

        existingEmployee.TechStack = employee.TechStack;
        existingEmployee.Address = employee.Address;
        existingEmployee.User.Phone = employee.User.Phone;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ResetPasswordAsync(string email, string newPassword)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        if (user == null) return false;

        user.PasswordHash = HashPassword(newPassword);
        await _context.SaveChangesAsync();
        return true;
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(bytes);
    }

    public async Task<List<EmployeeDTO>> GetAllEmployeesAsync()
    {
        try
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .Include(e => e.User)
                    .ThenInclude(u => u.Role)  
                .Include(e => e.Timesheets)  
                .Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    FullName = $"{e.User.FirstName} {e.User.LastName}", 
                    Email = e.User.Email,
                    Department = e.Department.DepartmentName, 
                    TechStack = e.TechStack,
                    Address = e.Address,
                    DateOfBirth = e.DateOfBirth,
                    Timesheets = e.Timesheets.Select(t => new TimesheetDTO
                    {
                        Date = t.Date,
                        TotalHours = t.TotalHours
                    }).ToList()
                })
                .AsNoTracking()  
                .ToListAsync();

            return employees;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching employees: {ex.Message}");
            return new List<EmployeeDTO>();  
        }
    }
        public async Task<Employee> GetEmployeeByIdAsync(int employeeId)
    {
        return await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
    }

}

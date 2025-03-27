using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.Repository;
using Microsoft.EntityFrameworkCore;

public class DepartmentRepository : IDepartmentRepository
{
    private readonly AppDbContext _context;

    public DepartmentRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
    {
        return await _context.Departments
            .Include(d => d.Employees) 
            .ThenInclude(e => e.User)  
            .ToListAsync();
    }

    public async Task<Department?> GetDepartmentByIdAsync(int id)
    {
        return await _context.Departments.Include(d => d.Employees).FirstOrDefaultAsync(d => d.DepartmentId == id);
    }

    public async Task<int> AssignEmployeeToDepartmentAsync(int userId, int departmentId, int? employeeId)
    {
        if (employeeId == null)
        {
            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
            {
                Console.WriteLine($"User with ID {userId} does not exist.");
                return 0; 
            }

            var newEmployee = new Employee
            {
                UserId = userId,
                DepartmentId = departmentId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _context.Employees.AddAsync(newEmployee);
            await _context.SaveChangesAsync();
            return newEmployee.EmployeeId; 
        }
        else
        {
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.EmployeeId == employeeId);
            if (employee == null)
            {
                Console.WriteLine($"Employee with ID {employeeId} not found.");
                return 0; 
            }

            employee.DepartmentId = departmentId;
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return employee.EmployeeId; 
        }
    }

    public async Task<bool> CreateDepartmentAsync(Department department)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                throw new ArgumentException("Department name is required.");

            if (await _context.Departments.AnyAsync(d => d.DepartmentName == department.DepartmentName))
                throw new ArgumentException("Department with the same name already exists.");

            department.CreatedAt = DateTime.UtcNow;

            await _context.Departments.AddAsync(department);
            await _context.SaveChangesAsync();
            return true;
        }
        catch (ArgumentException ex)
        {
            throw; 
        }
        catch (Exception ex)
        {
            return false;
        }
    }

    public async Task<bool> UpdateDepartmentAsync(Department department)
    {
        if (string.IsNullOrWhiteSpace(department.DepartmentName))
            throw new ArgumentException("Department name is required.");

        var existingDept = await _context.Departments.FindAsync(department.DepartmentId);
        if (existingDept == null)
            throw new ArgumentException("Department not found.");

        if (existingDept.DepartmentName == department.DepartmentName)
            throw new ArgumentException("Department with the same name already exists.");

        existingDept.DepartmentName = department.DepartmentName;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteDepartmentAsync(int departmentId)
    {
        if (departmentId <= 0)
            throw new ArgumentException("Invalid department ID.");

        var department = await _context.Departments.FindAsync(departmentId);
        if (department == null)
            throw new ArgumentException("Department not found.");

        _context.Departments.Remove(department);
        await _context.SaveChangesAsync();
        return true;
    }

}

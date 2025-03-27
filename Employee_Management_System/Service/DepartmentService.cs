using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.Repository;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;
        private readonly AppDbContext _context;

        public DepartmentService(IDepartmentRepository departmentRepository, AppDbContext context)
        {
            _departmentRepository = departmentRepository;
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartmentsAsync()
        {
            return await _departmentRepository.GetAllDepartmentsAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.Employees)
                .FirstOrDefaultAsync(d => d.DepartmentId == id);
        }

        public async Task<int> AssignEmployeeToDepartmentAsync(int userId, int departmentId, int? employeeId)
        {
            return await _departmentRepository.AssignEmployeeToDepartmentAsync(userId, departmentId, employeeId);
        }

        public async Task<bool> CreateDepartmentAsync(Department department)
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

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            return await _departmentRepository.UpdateDepartmentAsync(department);
        }

        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            return await _departmentRepository.DeleteDepartmentAsync(departmentId);
        }

        public async Task<Dictionary<string, int>> GetDepartmentWiseEmployeeCountAsync()
        {
            return await _context.Departments.Include(d => d.Employees)
                .ToDictionaryAsync(d => d.DepartmentName, d => d.Employees.Count);
        }

    }
}

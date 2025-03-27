using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Repository
{
    public interface IDepartmentRepository
    {
        Task<IEnumerable<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<int> AssignEmployeeToDepartmentAsync(int userId, int departmentId, int? employeeId);
        Task<bool> CreateDepartmentAsync(Department department);
        Task<bool> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int departmentId);
    }
}

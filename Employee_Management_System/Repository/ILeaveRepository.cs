using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Repository
{
    public interface ILeaveRepository
    {
        Task<IEnumerable<Leave>> GetAllLeavesAsync();
        Task<IEnumerable<Leave>> GetLeavesByEmployeeIdAsync(int employeeId);
        Task<Leave?> GetLeaveByIdAsync(int leaveId);
        Task<bool> ApplyLeaveAsync(Leave leave);
        Task<bool> UpdateLeaveStatusAsync(int leaveId, string status);
    }
}

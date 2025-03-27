using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Service
{
    public interface ILeaveService
    {
        Task<IEnumerable<Leave>> GetAllLeavesAsync();
        Task<IEnumerable<Leave>> GetEmployeeLeavesAsync(int employeeId);
        Task<bool> ApplyLeaveAsync(int employeeId, Leave leave);
        Task<bool> UpdateLeaveStatusAsync(int leaveId, string status);

        Task<int> GetPendingLeaveCountAsync();
        Task<int> GetLeaveBalanceAsync(int employeeId);

    }
}

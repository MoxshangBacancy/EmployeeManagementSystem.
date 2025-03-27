using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Employee_Management_System.Service
{
    public class LeaveService : ILeaveService
    {
        private readonly ILeaveRepository _leaveRepository;
        private readonly ILogger<LeaveService> _logger;
        private readonly AppDbContext _context;
        private readonly IEmployeeRepository _employeeRepository;

        public LeaveService(ILeaveRepository leaveRepository, ILogger<LeaveService> logger, AppDbContext context, IEmployeeRepository employeeRepository)
        {
            _leaveRepository = leaveRepository;
            _logger = logger;
            _context = context;
            _employeeRepository = employeeRepository;
        }

        public async Task<IEnumerable<Leave>> GetAllLeavesAsync()
        {
            return await _leaveRepository.GetAllLeavesAsync();
        }

        public async Task<IEnumerable<Leave>> GetEmployeeLeavesAsync(int employeeId)
        {
            var leaves = await _leaveRepository.GetLeavesByEmployeeIdAsync(employeeId);

            if (leaves == null || !leaves.Any())
            {
                throw new InvalidOperationException("No leave requests found for this employee.");
            }

            return leaves;
        }


        public async Task<bool> ApplyLeaveAsync(int employeeId, Leave leave)
        {
            try
            {
                var employee = await _employeeRepository.GetEmployeeByIdAsync(employeeId);
                if (employee == null)
                {
                    _logger.LogWarning($"Leave application failed: Employee ID {employeeId} does not exist.");
                    return false;
                }

                leave.EmployeeId = employeeId;
                leave.AppliedAt = DateTime.UtcNow;
                leave.Status = "Pending"; 

                bool isApplied = await _leaveRepository.ApplyLeaveAsync(leave);
                return isApplied;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error applying for leave for Employee ID {employeeId}");
                return false;
            }
        }


        public async Task<bool> UpdateLeaveStatusAsync(int leaveId, string status)
        {
            var leaveRequest = await _leaveRepository.GetLeaveByIdAsync(leaveId);
            if (leaveRequest == null)
                throw new ArgumentException("Leave request not found.");

            if (leaveRequest.Status == status)
                throw new ArgumentException($"Leave request is already {status}.");

            if (leaveRequest.Status == "Rejected" && (status == "Approved" || status == "Pending"))
                throw new ArgumentException("A rejected leave request cannot be changed.");

            if (leaveRequest.Status == "Approved" && (status == "Pending" || status == "Rejected"))
                throw new ArgumentException("An approved leave request cannot be changed.");

            return await _leaveRepository.UpdateLeaveStatusAsync(leaveId, status);
        }

        public async Task<int> GetPendingLeaveCountAsync()
        {
            return await _context.Leaves.CountAsync(l => l.Status == "Pending");
        }

        public async Task<int> GetLeaveBalanceAsync(int employeeId)
        {
            return 20 - await _context.Leaves.CountAsync(l => l.EmployeeId == employeeId); 
        }
    }
}

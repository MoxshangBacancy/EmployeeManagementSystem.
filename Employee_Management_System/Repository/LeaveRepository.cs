using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Repository
{
    public class LeaveRepository : ILeaveRepository
    {
        private readonly AppDbContext _context;

        public LeaveRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Leave>> GetAllLeavesAsync()
        {
            return await _context.Leaves
                .Include(l => l.Employee)
                .ThenInclude(e => e.User) 
                .ToListAsync();
        }
        public async Task<IEnumerable<Leave>> GetLeavesByEmployeeIdAsync(int employeeId)
        {
            return await _context.Leaves.Where(l => l.EmployeeId == employeeId).ToListAsync();
        }

        public async Task<Leave?> GetLeaveByIdAsync(int leaveId)
        {
            return await _context.Leaves.Include(l => l.Employee).FirstOrDefaultAsync(l => l.LeaveId == leaveId);
        }

        public async Task<bool> ApplyLeaveAsync(Leave leave)
        {
            _context.Leaves.Add(leave);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateLeaveStatusAsync(int leaveId, string status)
        {
            var leave = await _context.Leaves.FindAsync(leaveId);
            if (leave == null) return false;

            leave.Status = status;
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

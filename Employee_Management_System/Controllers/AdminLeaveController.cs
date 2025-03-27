using Employee_Management_System.DTOs;
using Employee_Management_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    [Route("Api/Admin/")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class AdminLeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public AdminLeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        // Get All Leave Requests
        [HttpGet("GetAllLeaves")]
        public async Task<IActionResult> GetAllLeaveRequests()
        {
            try
            {
                var leaves = await _leaveService.GetAllLeavesAsync();
                if (!leaves.Any()) return NotFound(new { message = "No leave requests found." });

                var leaveDTOs = leaves.Select(l => new LeaveDTO
                {
                    LeaveId = l.LeaveId,
                    EmployeeId = l.EmployeeId,
                    EmployeeName = l.Employee?.User != null
                        ? $"{l.Employee.User.FirstName} {l.Employee.User.LastName}"
                        : "Unknown Employee",
                    StartDate = l.StartDate,
                    EndDate = l.EndDate,
                    LeaveType = l.LeaveType,
                    Reason = l.Reason,
                    Status = l.Status,
                    AppliedAt = l.AppliedAt
                }).ToList();

                return Ok(leaveDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error fetching leave requests.", error = ex.Message });
            }
        }

        // Approve/Reject Leave Request
        [HttpPut("ChangeStatus/{leaveId}")]
        public async Task<IActionResult> UpdateLeaveStatus(int leaveId, [FromBody] string status)
        {
            try
            {
                if (status != "Approved" && status != "Rejected" && status != "Pending")
                    return BadRequest(new { message = "Invalid status. Use 'Approved', 'Rejected', or 'Pending'." });

                var result = await _leaveService.UpdateLeaveStatusAsync(leaveId, status);
                return Ok(new { message = $"Leave request has been {status.ToLower()} successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the leave status. Please try again later." });
            }
        }
    }
}

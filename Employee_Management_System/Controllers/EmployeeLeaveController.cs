using Employee_Management_System.Data.Entities;
using Employee_Management_System.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Employee_Management_System.Request;

namespace Employee_Management_System.Controllers
{
    [Route("Api/Employee")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    public class EmployeeLeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public EmployeeLeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        // Get Leave request of Logged-in Employee
        [HttpGet("GetMyLeaves")]
        public async Task<IActionResult> GetMyLeaveRequests()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email)) return Unauthorized(new { message = "Invalid token or session expired." });

                var employeeIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(employeeIdClaim) || !int.TryParse(employeeIdClaim, out int employeeId))
                {
                    return Unauthorized(new { message = "Invalid token or user ID." });
                }

                var leaves = await _leaveService.GetEmployeeLeavesAsync(employeeId);

                return Ok(leaves);
            }
            catch (InvalidOperationException ex) 
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An internal error occurred. Please try again later." });
            }
        }


        // Apply for Leave
        [HttpPost("ApplyForLeave")]
        public async Task<IActionResult> ApplyForLeave([FromBody] LeaveRequestDTO request)
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                if (string.IsNullOrEmpty(email))
                    return Unauthorized(new { message = "Invalid token or session expired." });

                var employeeId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

                var leave = new Leave
                {
                    EmployeeId = employeeId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    LeaveType = request.LeaveType,
                    Reason = request.Reason,
                    AppliedAt = DateTime.UtcNow,
                    Status = "Pending"
                };

                var result = await _leaveService.ApplyLeaveAsync(employeeId, leave);
                if (!result) return BadRequest(new { message = "Failed to apply for leave." });

                return Ok(new { message = "Leave request submitted successfully." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Error processing leave request.", error = ex.Message });
            }
        }
    }
}

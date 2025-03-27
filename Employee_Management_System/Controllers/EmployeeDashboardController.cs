using Employee_Management_System.DTOs;
using Employee_Management_System.Service;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    [Route("Api/")]
    [ApiController]
    [Authorize]
    public class EmployeeDashboardController : ControllerBase
    {
        private readonly ITimesheetService _timesheetService;
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;

        public EmployeeDashboardController(
            ITimesheetService timesheetService,
            ILeaveService leaveService,
            IEmployeeService employeeService,
            IDepartmentService departmentService)
        {
            _timesheetService = timesheetService;
            _leaveService = leaveService;
            _employeeService = employeeService;
        }


        // Employee Dashboard
        [HttpGet("EmployeeDashboard")]
        [Authorize(Roles = "Employee")]
        public async Task<IActionResult> GetEmployeeDashboard()
        {
            try
            {
                var email = User.FindFirst(ClaimTypes.Email)?.Value;
                Console.WriteLine($"🔹 Extracted Email from JWT: {email}");

                if (string.IsNullOrEmpty(email))
                    return Unauthorized(new { message = "Invalid token or session expired." });

                var employee = await _employeeService.GetEmployeeProfileAsync(email);
                if (employee == null)
                {
                    Console.WriteLine($"❌ Employee not found for email: {email}");
                    return NotFound(new { message = "Employee not found." });
                }

                Console.WriteLine($"✅ Employee found: {employee.EmployeeId}");

                var totalHours = await _timesheetService.GetTotalLoggedHoursAsync(employee.EmployeeId);
                var leaveBalance = await _leaveService.GetLeaveBalanceAsync(employee.EmployeeId);
                var recentTimesheets = await _timesheetService.GetRecentTimesheetsAsync(employee.EmployeeId, 5);

                var response = new EmployeeDashboardDTO
                {
                    TotalLoggedHours = totalHours,
                    LeaveBalance = leaveBalance,
                    RecentTimesheets = recentTimesheets.Select(t => new TimesheetDTO
                    {
                        TimesheetId = t.TimesheetId,
                        Date = t.Date,
                        StartTime = t.StartTime.ToString(),
                        EndTime = t.EndTime.ToString(),
                        TotalHours = t.TotalHours,
                        Description = t.Description
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching dashboard data.", error = ex.Message });
            }
        }
    }
}


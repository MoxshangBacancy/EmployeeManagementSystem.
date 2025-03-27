using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs;
using Employee_Management_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("Api/Timesheets")]
[ApiController]
[Authorize(Roles = "Employee")]
public class EmployeeTimesheetController : ControllerBase
{
    private readonly ITimesheetService _timesheetService;
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeeTimesheetController> _logger;

    public EmployeeTimesheetController(ITimesheetService timesheetService, IEmployeeService employeeService, ILogger<EmployeeTimesheetController> logger)
    {
        _timesheetService = timesheetService;
        _employeeService = employeeService;
        _logger = logger;
    }

    // Fetch Logged-in Employee's Timesheets
    [HttpGet("GetMyRecords")]
    public async Task<IActionResult> GetMyTimesheetRecords()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value; 
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Invalid token or session expired." });

            var employee = await _employeeService.GetEmployeeProfileAsync(email);
            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            var timesheets = await _timesheetService.GetEmployeeTimesheetsAsync(employee.EmployeeId);

            var timesheetDTOs = timesheets.Select(t => new TimesheetDTO
            {
                TimesheetId = t.TimesheetId,
                Date = t.Date,
                StartTime = t.StartTime.ToString(),
                EndTime = t.EndTime.ToString(),
                TotalHours = t.TotalHours,
                Description = t.Description ?? "No Description"
            }).ToList();

            return Ok(timesheetDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving timesheet records.");
            return StatusCode(500, new { message = "An error occurred while fetching timesheet records. Please try again later." });
        }
    }

    // Log Work Hours
    [HttpPost("LogWorksHours")]
    public async Task<IActionResult> LogWorkHours([FromBody] Employee_Management_System.Data.Entities.TimesheetRequest request)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Invalid token or session expired." });

            var employee = await _employeeService.GetEmployeeProfileAsync(email);
            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            var result = await _timesheetService.AddTimesheetAsync(employee.EmployeeId, request);
            return result.IsSuccess
                ? Ok(new { message = result.Message })
                : BadRequest(new { message = result.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error logging work hours.");
            return StatusCode(500, new { message = "An error occurred while logging work hours. Please try again later." });
        }
    }

}

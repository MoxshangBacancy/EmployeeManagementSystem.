using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

[Route("Api/Employees")]
[ApiController]
[Authorize(Roles = "Employee")] 
public class EmployeeProfileController : ControllerBase
{
    private readonly IEmployeeService _employeeService;
    private readonly ILogger<EmployeeProfileController> _logger;

    public EmployeeProfileController(IEmployeeService employeeService, ILogger<EmployeeProfileController> logger)
    {
        _employeeService = employeeService;
        _logger = logger;
    }

    // Get Employee Profile
    [HttpGet("MyProfile")]
    public async Task<IActionResult> GetEmployeeProfile()
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Invalid token or session expired." });

            var employee = await _employeeService.GetEmployeeProfileAsync(email);
            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            var employeeDTO = new EmployeeProfileDTO
            {
                EmployeeId = employee.EmployeeId,
                FullName = $"{employee.User.FirstName} {employee.User.LastName}",
                Email = employee.User.Email,
                Phone = employee.User.Phone,
                TechStack = employee.TechStack,
                Address = employee.Address,
                Department = employee.Department?.DepartmentName ?? "Not Assigned",
                DateOfBirth = employee.DateOfBirth,
                IsActive = employee.User.IsActive,
            };

            return Ok(employeeDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching employee profile.");
            return StatusCode(500, new { message = "An error occurred while retrieving your profile. Please try again later." });
        }
    }

    // Update Employee Profile (Phone, Tech Stack, Address)
    [HttpPut("UpdateProfile")]
    public async Task<IActionResult> UpdateEmployeeProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            var email = User.FindFirst(ClaimTypes.Email)?.Value;
            if (string.IsNullOrEmpty(email))
                return Unauthorized(new { message = "Invalid token or session expired." });

            var employee = await _employeeService.GetEmployeeProfileAsync(email);
            if (employee == null)
                return NotFound(new { message = "Employee not found." });

            employee.User.Phone = request.Phone;
            employee.TechStack = request.TechStack;
            employee.Address = request.Address;

            var result = await _employeeService.UpdateEmployeeProfileAsync(employee);
            if (!result)
                return BadRequest(new { message = "Profile update failed." });

            return Ok(new { message = "Your profile has been updated successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating employee profile.");
            return StatusCode(500, new { message = "An error occurred while updating your profile. Please try again later." });
        }
    }
}

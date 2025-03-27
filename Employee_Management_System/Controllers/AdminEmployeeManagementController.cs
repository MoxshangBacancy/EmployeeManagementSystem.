using Employee_Management_System.DTOs;
using Employee_Management_System.Request;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    [Route("Api/Admin/")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminEmployeeManagementController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminEmployeeManagementController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        // Get Employees with their Timesheets
        [HttpGet("GetEmployeesWithTimesheets")]
        public async Task<IActionResult> GetAllEmployees()
        {
            try
            {
                var employees = await _adminService.GetAllEmployeesAsync();
                if (!employees.Any()) return NotFound(new { message = "No employees found." });

                var employeeDTOs = employees.Select(e => new EmployeeDTO
                {
                    EmployeeId = e.EmployeeId,
                    FullName = $"{e.User.FirstName} {e.User.LastName}",
                    Email = e.User.Email,
                    Department = e.Department?.DepartmentName ?? "Not Assigned",
                    TechStack = e.TechStack,
                    Address = e.Address,
                    DateOfBirth = e.DateOfBirth,
                    Timesheets = e.Timesheets.Select(t => new TimesheetDTO
                    {
                        TimesheetId = t.TimesheetId,
                        Date = t.Date,
                        StartTime = t.StartTime.ToString(),
                        EndTime = t.EndTime.ToString(),
                        TotalHours = t.TotalHours,
                        Description = t.Description
                    }).ToList()
                });

                return Ok(employeeDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while fetching employees. Please try again later." });
            }
        }

        // Update Employee Details
        [HttpPut("UpdateEmployees/{id}")]
        public async Task<IActionResult> UpdateEmployeeDetails(int id, [FromBody] EmployeeUpdateRequest request)
        {
            try
            {
                var result = await _adminService.UpdateEmployeeAsync(id, request);
                if (!result)
                    return BadRequest(new { message = "Employee not found or update failed." });

                return Ok(new { message = "Employee profile updated successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while updating the employee profile. Please try again later." });
            }
        }


        // Delete Employee by ID
        [HttpDelete("DeleteEmployee/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var result = await _adminService.DeleteEmployeeAsync(id);

                return Ok(new { message = "Employee deleted successfully." });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while deleting the employee. Please try again later." });
            }
        }
    }
}

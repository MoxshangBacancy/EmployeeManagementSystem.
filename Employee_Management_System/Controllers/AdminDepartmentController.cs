using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs;
using Employee_Management_System.Request;
using Employee_Management_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Route("Api/Departments")]
[ApiController]
[Authorize(Roles = "Admin")] 
public class AdminDepartmentController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    public AdminDepartmentController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    // Get All Departments with Employee Details
    [HttpGet("GetAllWithEmploy")]
    public async Task<IActionResult> GetAllDepartmentsWithEmployees()
    {
        var departments = await _departmentService.GetAllDepartmentsAsync();

        var departmentDTOs = departments.Select(d => new DepartmentDTO
        {
            DepartmentId = d.DepartmentId,
            DepartmentName = d.DepartmentName,
            CreatedAt = d.CreatedAt,
            Employees = d.Employees.Select(e => new EmployeeProfileDTO
            {
                EmployeeId = e.EmployeeId,
                FullName = $"{e.User.FirstName} {e.User.LastName}",
                Email = e.User.Email,
                Phone = e.User.Phone,
                TechStack = e.TechStack,
                Address = e.Address,
                Department = d.DepartmentName, 
                DateOfBirth = e.DateOfBirth,
                IsActive = e.User.IsActive
            }).ToList()
        }).ToList();

        return Ok(departmentDTOs);
    }

    // Assign Employee to a Department
    [HttpPost("AssignEmployee")]
    public async Task<IActionResult> AssignEmployeeToDepartment([FromBody] AssignDepartmentRequest request)
    {
        int? employeeId = request.EmployeeId == 0 ? null : request.EmployeeId;

        int result = await _departmentService.AssignEmployeeToDepartmentAsync(request.UserId, request.DepartmentId, request.EmployeeId);

        if (result == 0)
            return NotFound(new { message = "User not found." });

        if (result == -1)
            return BadRequest(new { message = "Employee exists. Please provide EmployeeId to change department." });

        return Ok(new
        {
            message = "Employee successfully assigned/updated in department.",
            EmployeeId = result
        });
    }

    // Create a Department
    [HttpPost("Create")]
    public async Task<IActionResult> CreateDepartment([FromBody] DepartmentCreateDTO request)
    {
        try
        {
            var department = new Department
            {
                DepartmentName = request.DepartmentName
            };

            var result = await _departmentService.CreateDepartmentAsync(department);
            if (!result)
                return BadRequest(new { message = "Failed to create department." });

            return Ok(new { message = "Department created successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while creating the department. Please try again later." });
        }
    }

    // Update Department Name
    [HttpPut("Update/{id}")]
    public async Task<IActionResult> UpdateDepartmentName(int id, [FromBody] DepartmentUpdateDTO request)
    {
        try
        {
            var department = new Department
            {
                DepartmentId = id,
                DepartmentName = request.DepartmentName
            };

            var result = await _departmentService.UpdateDepartmentAsync(department);

            if (!result)
                return BadRequest(new { message = "Failed to update department name." });

            return Ok(new { message = "Department name updated successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message }); 
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while updating the department. Please try again later." });
        }
    }

    // Delete a Department
    [HttpDelete("Remove/{id}")]
    public async Task<IActionResult> RemoveDepartment(int id)
    {
        try
        {
            var result = await _departmentService.DeleteDepartmentAsync(id);
                
            if (!result)
                return BadRequest(new { message = "Failed to remove department." });

            return Ok(new { message = "Department removed successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "An error occurred while removing the department. Please try again later." });
        }
    }
}

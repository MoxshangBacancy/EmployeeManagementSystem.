using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs;
using Employee_Management_System.Request;
using Employee_Management_System.Service;
using Employee_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

[Route("Api/Admin")]
[ApiController]
[Authorize(Roles = "Admin")]
public class AdminUserManagementController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AdminUserManagementController> _logger;

    public AdminUserManagementController(IUserService userService, ILogger<AdminUserManagementController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    // Get All Users
    [HttpGet("GetAllUsers")]
    public async Task<IActionResult> GetAllUsers()
    {
        try
        {
            var users = await _userService.GetAllUsersAsync();
            if (!users.Any()) return NotFound(new { message = "No users found." });

            var userDTOs = users.Select(u => new UserDTO
            {
                Id = u.Id,
                FullName = $"{u.FirstName} {u.LastName}",
                Email = u.Email,
                Phone = u.Phone,
                Role = u.Role?.RoleName ?? "Not Assigned",
                IsActive = u.IsActive,
                CreatedAt = u.CreatedAt,
                UpdatedAt = u.UpdatedAt
            }).ToList();

            return Ok(userDTOs);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error fetching users.");
            return StatusCode(500, new { message = "An error occurred while fetching users. Please try again later." });
        }
    }

    // Get User by ID
    [HttpGet("GetUserById/{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            var userDTO = new UserDTO
            {
                Id = user.Id,
                FullName = $"{user.FirstName} {user.LastName}",
                Email = user.Email,
                Phone = user.Phone,
                Role = user.Role?.RoleName ?? "Not Assigned",
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };

            return Ok(userDTO);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error fetching user {id}.");
            return StatusCode(500, new { message = "An error occurred while fetching the user. Please try again later." });
        }
    }

    // Create a New User
    [HttpPost("CreateUser")]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request)
    {
        try
        {
            var user = new User
            {
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                Phone = request.Phone,
                PasswordHash = _userService.HashPassword(request.Password),
                RoleId = request.RoleId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _userService.CreateUserAsync(user);
            return Ok(new { message = "User created successfully." });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message }); 
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user.");
            return StatusCode(500, new { message = "An error occurred while creating the user. Please try again later." });
        }
    }


    // Update User Details
    [HttpPut("UpdateUser/{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRequest request)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null) return NotFound(new { message = "User not found." });

            user.FirstName = request.FirstName ?? user.FirstName;
            user.LastName = request.LastName ?? user.LastName;
            user.Phone = request.Phone ?? user.Phone;
            user.RoleId = request.RoleId ?? user.RoleId;
            user.IsActive = request.IsActive ?? user.IsActive;
            user.UpdatedAt = DateTime.UtcNow;

            var result = await _userService.UpdateUserAsync(user);
            if (!result) return BadRequest(new { message = "Failed to update user." });

            return Ok(new { message = "User updated successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error updating user {id}.");
            return StatusCode(500, new { message = "An error occurred while updating the user. Please try again later." });
        }
    }

    // Delete User
    [HttpDelete("DeleteUser/{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            var result = await _userService.DeleteUserAsync(id);
            if (!result) return NotFound(new { message = "User not found." });

            return Ok(new { message = "User deleted successfully." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error deleting user {id}.");
            return StatusCode(500, new { message = "An error occurred while deleting the user. Please try again later." });
        }
    }
}

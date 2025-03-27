using Employee_Management_System.DTOs;
using Employee_Management_System.Service;
using Employee_Management_System.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Employee_Management_System.Controllers
{
    [Route("Api/Admin/")]
    [ApiController]
    [Authorize(Roles = "Admin")] 
    public class AdminReportController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ILogger<AdminReportController> _logger;
        private readonly ILeaveService _leaveService;
        private readonly IEmployeeService _employeeService;
        private readonly IDepartmentService _departmentService;

        public AdminReportController(IAdminService adminService, ILogger<AdminReportController> logger, ILeaveService leaveService,
            IEmployeeService employeeService,
            IDepartmentService departmentService)
        {
            _adminService = adminService;
            _logger = logger;
            _leaveService = leaveService;
            _employeeService = employeeService;
            _departmentService = departmentService;
        }

        [HttpGet("Report/Work-Hours")]
        public async Task<IActionResult> GenerateWorkHoursReport([FromQuery] string periodType, [FromQuery] int year, [FromQuery] int monthOrWeek)
        {
            var result = await _adminService.GetEmployeeWorkHoursReportAsync(periodType, year, monthOrWeek);

            if (!result.Any())
                return NotFound(new { message = "No work hours data found." });

            return Ok(result);
        }


        // Admin Dashboard
        [HttpGet("Dashboard")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminDashboard()
        {
            try
            {
                var totalEmployees = await _employeeService.GetTotalEmployeesAsync();

                var activeEmployees = await _employeeService.GetActiveEmployeesAsync();

                var pendingLeaves = await _leaveService.GetPendingLeaveCountAsync();

                var departmentWiseCount = await _departmentService.GetDepartmentWiseEmployeeCountAsync();

                var response = new AdminDashboardDTO
                {
                    TotalEmployees = totalEmployees,
                    ActiveEmployees = activeEmployees,
                    PendingLeaveRequests = pendingLeaves,
                    DepartmentWiseEmployeeCount = departmentWiseCount
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

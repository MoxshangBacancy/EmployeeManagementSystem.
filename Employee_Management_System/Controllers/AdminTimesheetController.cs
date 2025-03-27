using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs;
using Employee_Management_System.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficeOpenXml;

namespace Employee_Management_System.Controllers
{
    [Route("Api/Admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminTimesheetController : ControllerBase
    {
        private readonly IAdminService _adminService;
        private readonly ITimesheetService _timesheetService;
        private readonly ILogger<AdminTimesheetController> _logger;

        public AdminTimesheetController(IAdminService adminService, ITimesheetService timesheetService, ILogger<AdminTimesheetController> logger)
        {
            _adminService = adminService;
            _timesheetService = timesheetService;
            _logger = logger;
        }

        // Get All Timesheets for an Employee
        [HttpGet("TimesheetForEmployee/{employeeId}")]
        public async Task<IActionResult> GetEmployeeTimesheets(int employeeId)
        {
            try
            {
                var timesheets = await _adminService.GetEmployeeTimesheetsAsync(employeeId);
                if (!timesheets.Any()) return NotFound(new { message = "No timesheets found for this employee." });

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
                _logger.LogError(ex, $"Error fetching timesheets for employee {employeeId}.");
                return StatusCode(500, new { message = "An error occurred while fetching timesheets. Please try again later." });
            }
        }

        // Export All Employee Timesheets to Excel
        [HttpGet("AllEmployee/ExcelReport")]
        [ResponseCache(NoStore = true, Location = ResponseCacheLocation.None)]

        public async Task<IActionResult> DownloadEmployeeTimesheets()
        {
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

                var timesheets = await _adminService.GetAllTimesheetsAsync();
                if (!timesheets.Any()) return NotFound(new { message = "No timesheets found." });

                using var package = new ExcelPackage();
                var worksheet = package.Workbook.Worksheets.Add("Timesheets");

                worksheet.Cells["A1"].Value = "Employee ID";
                worksheet.Cells["B1"].Value = "Date";
                worksheet.Cells["C1"].Value = "Start Time";
                worksheet.Cells["D1"].Value = "End Time";
                worksheet.Cells["E1"].Value = "Total Hours";
                worksheet.Cells["F1"].Value = "Description";

                int row = 2;
                foreach (var timesheet in timesheets)
                {
                    worksheet.Cells[row, 1].Value = timesheet.EmployeeId;
                    worksheet.Cells[row, 2].Value = timesheet.Date.ToString("yyyy-MM-dd");
                    worksheet.Cells[row, 3].Value = timesheet.StartTime.ToString();
                    worksheet.Cells[row, 4].Value = timesheet.EndTime.ToString();
                    worksheet.Cells[row, 5].Value = timesheet.TotalHours;
                    worksheet.Cells[row, 6].Value = timesheet.Description;
                    row++;
                }   

                var stream = new MemoryStream();
                package.SaveAs(stream);
                stream.Position = 0;

                return File(stream, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Employee_Timesheets.xlsx");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting timesheets.");
                return StatusCode(500, new { message = "An error occurred while exporting timesheets. Please try again later." });
            }
        }

        // Modify Work Hours
        [HttpPut("UpdateTime/{id}")]
        public async Task<IActionResult> ModifyWorkHours(int id, [FromBody] Data.Entities.TimesheetRequest request)
        {
            try
            {
                var result = await _timesheetService.UpdateTimesheetAsync(id, request);
                if (!result)
                    return NotFound(new { message = "Timesheet record not found or update failed." });

                return Ok(new { message = "Work hours updated successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating work hours.");
                return StatusCode(500, new { message = "An error occurred while updating work hours. Please try again later." });
            }
        }
    }

}

using Azure.Core;
using Employee_Management_System.Data;
using Employee_Management_System.Data.Entities;
using Employee_Management_System.DTOs;
using Employee_Management_System.Repository;
using Employee_Management_System.Request;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Services
{
    public class AdminService : IAdminService
    {
        private readonly IAdminRepository _adminRepository;

        public AdminService(IAdminRepository adminRepository)
        {
            _adminRepository = adminRepository;
        }
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await _adminRepository.GetAllEmployeesWithTimesheetsAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _adminRepository.GetEmployeeByIdAsync(id);
        }

        public async Task<bool> UpdateEmployeeAsync(int id, EmployeeUpdateRequest request)
        {
            var employee = await _adminRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
                throw new ArgumentException("Employee not found.");

            if (!employee.User.IsActive)
                throw new ArgumentException("Employee is not active. Updates are not allowed.");

            if (!string.IsNullOrWhiteSpace(request.Phone) && request.Phone != employee.User.Phone)
            {
                bool isPhoneExists = await _adminRepository.IsPhoneNumberExistsAsync(request.Phone, id);
                if (isPhoneExists)
                    throw new ArgumentException("Phone number is already in use.");

                employee.User.Phone = request.Phone;
            }

            if (!string.IsNullOrWhiteSpace(request.TechStack))
                employee.TechStack = request.TechStack;

            if (!string.IsNullOrWhiteSpace(request.Address))
                employee.Address = request.Address;

            return await _adminRepository.UpdateEmployeeAsync(employee, request.IsActive);
        }

        public async Task<IEnumerable<Timesheet>> GetEmployeeTimesheetsAsync(int employeeId)
        {
            return await _adminRepository.GetEmployeeTimesheetsAsync(employeeId);
        }

        public async Task<IEnumerable<Timesheet>> GetAllTimesheetsAsync()
        {
            return await _adminRepository.GetAllTimesheetsAsync();
        }
        public async Task<IEnumerable<WorkHoursReportDTO>> GetEmployeeWorkHoursReportAsync(string periodType, int year, int monthOrWeek)
        {
            if (string.IsNullOrEmpty(periodType) || (periodType.ToLower() != "weekly" && periodType.ToLower() != "monthly"))
            {
                throw new ArgumentException("Invalid periodType. Use 'weekly' or 'monthly'.");
            }

            if (year < 2000 || year > DateTime.Now.Year)
            {
                throw new ArgumentException("Invalid year.");
            }

            if (periodType.ToLower() == "monthly" && (monthOrWeek < 1 || monthOrWeek > 12))
            {
                throw new ArgumentException("Invalid month. Must be between 1 and 12.");
            }

            if (periodType.ToLower() == "weekly" && (monthOrWeek < 1 || monthOrWeek > 53))
            {
                throw new ArgumentException("Invalid week number. Must be between 1 and 53.");
            }

            return await _adminRepository.GetEmployeeWorkHoursReportAsync(periodType, year, monthOrWeek);
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            var employee = await _adminRepository.GetEmployeeByIdAsync(id);
            if (employee == null)
                throw new ArgumentException("Employee not found.");

            if (employee.User.IsActive)
                throw new ArgumentException("Employee is still active. Cannot delete.");

            return await _adminRepository.DeleteEmployeeAsync(id);
        }


    }
}

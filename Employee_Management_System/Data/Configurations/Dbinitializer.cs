using System.Security.Cryptography;
using System.Text;
using Employee_Management_System.Data.Entities;

namespace Employee_Management_System.Data.Configurations
{
    public static class DbInitializer
    {
        public static void Initialize(AppDbContext context)
        {
            Console.WriteLine("DbInitializer: Seeding process started...");

            if (!context.Roles.Any())
            {
                Console.WriteLine("Seeding Roles...");
                var roles = new List<Role>
                {
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "Employee" }
                };
                context.Roles.AddRange(roles);
                context.SaveChanges();
            }

            var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin")?.RoleId ?? 1;
            var employeeRole = context.Roles.FirstOrDefault(r => r.RoleName == "Employee")?.RoleId ?? 2;

            if (!context.Users.Any())
            {
                Console.WriteLine("Seeding Users...");
                var users = new List<User>
                {
                    new User { FirstName = "Admin", LastName = "User", Email = "admin@example.com", Phone = "9876543210",
                        PasswordHash = HashPassword("Admin@123"), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, RoleId = adminRole },

                    new User { FirstName = "John", LastName = "Doe", Email = "john.doe@example.com", Phone = "1234567890",
                        PasswordHash = HashPassword("John@123"), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, RoleId = employeeRole },

                    new User { FirstName = "Jane", LastName = "Smith", Email = "jane.smith@example.com", Phone = "2345678901",
                        PasswordHash = HashPassword("Jane@123"), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow, RoleId = employeeRole }
                };
                context.Users.AddRange(users);
                context.SaveChanges();
            }

            if (!context.Departments.Any())
            {
                Console.WriteLine("Seeding Departments...");
                var departments = new List<Department>
                {
                    new Department { DepartmentName = "IT", CreatedAt = DateTime.UtcNow },
                    new Department { DepartmentName = "HR", CreatedAt = DateTime.UtcNow },
                    new Department { DepartmentName = "Finance", CreatedAt = DateTime.UtcNow }
                };
                context.Departments.AddRange(departments);
                context.SaveChanges();
            }

            if (!context.Employees.Any())
            {
                Console.WriteLine("Seeding Employees...");

                var johnUserId = context.Users.FirstOrDefault(u => u.Email == "john.doe@example.com")?.Id ?? 2;
                var janeUserId = context.Users.FirstOrDefault(u => u.Email == "jane.smith@example.com")?.Id ?? 3;

                var employees = new List<Employee>
                {
                    new Employee { UserId = johnUserId, DepartmentId = 1, TechStack = "C#, .NET, Angular",
                        Address = "123 Tech Street", DateOfBirth = new DateTime(1995, 6, 15), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },

                    new Employee { UserId = janeUserId, DepartmentId = 2, TechStack = "HR Management, Payroll",
                        Address = "456 HR Avenue", DateOfBirth = new DateTime(1992, 9, 22), CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
                };
                context.Employees.AddRange(employees);
                context.SaveChanges();
            }

            if (!context.Timesheets.Any())
            {
                Console.WriteLine("Seeding Timesheets...");
                var johnEmployeeId = context.Employees.FirstOrDefault(e => e.User.Email == "john.doe@example.com")?.EmployeeId ?? 1;
                var janeEmployeeId = context.Employees.FirstOrDefault(e => e.User.Email == "jane.smith@example.com")?.EmployeeId ?? 2;

                var timesheets = new List<Timesheet>
                {
                    new Timesheet { EmployeeId = johnEmployeeId, Date = DateTime.UtcNow.Date, StartTime = new TimeSpan(9, 0, 0),
                        EndTime = new TimeSpan(17, 0, 0), TotalHours = 8, Description = "Developed API endpoints", CreatedAt = DateTime.UtcNow },

                    new Timesheet { EmployeeId = janeEmployeeId, Date = DateTime.UtcNow.Date, StartTime = new TimeSpan(10, 0, 0),
                        EndTime = new TimeSpan(18, 0, 0), TotalHours = 8, Description = "Processed HR payroll", CreatedAt = DateTime.UtcNow }
                };
                context.Timesheets.AddRange(timesheets);
                context.SaveChanges();
            }

            if (!context.Leaves.Any())
            {
                Console.WriteLine("Seeding Leave Requests...");
                var johnEmployeeId = context.Employees.FirstOrDefault(e => e.User.Email == "john.doe@example.com")?.EmployeeId ?? 1;
                var janeEmployeeId = context.Employees.FirstOrDefault(e => e.User.Email == "jane.smith@example.com")?.EmployeeId ?? 2;

                var leaves = new List<Leave>
                {
                    new Leave { EmployeeId = johnEmployeeId, StartDate = DateTime.UtcNow.AddDays(-2), EndDate = DateTime.UtcNow.AddDays(1),
                        LeaveType = "Sick", Reason = "Fever and cold", Status = "Approved", AppliedAt = DateTime.UtcNow },

                    new Leave { EmployeeId = janeEmployeeId, StartDate = DateTime.UtcNow.AddDays(3), EndDate = DateTime.UtcNow.AddDays(7),
                        LeaveType = "Vacation", Reason = "Family vacation", Status = "Pending", AppliedAt = DateTime.UtcNow }
                };
                context.Leaves.AddRange(leaves);
                context.SaveChanges();
            }

            Console.WriteLine("Seeding completed successfully!");
        }
        private static string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}

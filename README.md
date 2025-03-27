# Employee Management System

## Overview
The **Employee Management System** is a .NET Core-based web API designed to manage employees, timesheets, and leave requests. It follows a multi-tiered architecture, implements Entity Framework Code-First, and uses JWT authentication for security. The system supports two roles: **Employee** and **Admin**.

## Features

### 1. User Roles
- **Employee**: Logs working hours, updates their profile, and applies for leave.
- **Admin**: Manages employee profiles, views/export timesheets, and generates reports.

### 2. Employee Details
Each employee has the following attributes:
- Employee ID (Unique Identifier assigned by Admin)
- First Name
- Last Name
- Email
- Phone Number
- Department (Assigned by Admin)
- Tech Stack (Skills & Technologies known)
- Date of Birth (Optional)
- Address (Optional)

### 3. Timesheet Management
- Employees can log daily working hours.
- Each entry includes:
  - Date
  - Start Time & End Time
  - Total Hours Worked
  - Work Description (Optional)
- Employees can view and edit their timesheet entries.

### 4. Admin Functionalities
- Admins log in using email and password.
- View all employees and timesheets.
- Export timesheets to Excel.
- Manage employee profiles (edit details, activate/deactivate accounts).

### 5. Profile Management
- Employees have a profile page displaying their details.
- Update phone number, tech stack, and address.
- Password reset functionality via email verification.

### 6. Leave Management (Additional Feature)
- Employees can apply for leave:
  - Start & End Date
  - Type of Leave (Sick Leave, Casual Leave, Vacation, etc.)
  - Reason (Optional)

### 7. Reports & Analytics (Additional Feature)
- Admins generate reports for:
  - Employee Work Hours (weekly, monthly)

### 8. Authentication & Security
- Users log in using email & password.
- Passwords are securely stored using hashing.
- JWT authentication ensures secure API access.
- Password reset via email verification.

### 9. Dashboard
- **Employees**: View total logged hours, leave balance, and latest timesheet entries.
- **Admins**: Get an overview of employee activities, pending leave requests, and analytics.

---

## Project Structure
```
Employee_Management_System
│── Controllers       # API Controllers for handling requests
│── Data             
│   │── Configurations  # Entity configurations
│   │── Entities        # Database models
│   │── AppDbContext.cs # EF Core Database Context
│── DTOs              # Data Transfer Objects
│── Migrations        # EF Core Migrations
│── Repository        # Business logic for data access
│── Request          
│── Service          # Business logic implementation
│── appsettings.json # Configuration settings
│── Program.cs       # Entry point of the application
│── Timesheets.xlsx  # Sample timesheet export file
```

---

## Installation & Setup
1. **Clone the Repository**
   ```sh
   git clone https://github.com/yourusername/Employee_Management_System.git
   cd Employee_Management_System
   ```
2. **Setup Database**
   - Update connection string in `appsettings.json`.
   - Apply migrations:
     ```sh
     dotnet ef database update
     ```
3. **Run the Application**
   ```sh
   dotnet run
   ```

---

## API Endpoints
### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - Register new employees (Admin only)
- `POST /api/auth/reset-password` - Password reset

### Employees
- `GET /api/employees` - Get all employees (Admin only)
- `GET /api/employees/{id}` - Get employee details
- `PUT /api/employees/{id}` - Update profile details
- `DELETE /api/employees/{id}` - Deactivate employee (Admin only)

### Timesheets
- `POST /api/timesheets` - Log working hours
- `GET /api/timesheets` - Get timesheets
- `PUT /api/timesheets/{id}` - Update entry

### Leave Management
- `POST /api/leave` - Apply for leave
- `GET /api/leave` - View leave requests
- `PUT /api/admin/changestatus/{leaveId}` - Change leave status (Admin only)

### Admin Management
- `GET /api/admin/getemployeeswithtimesheets` - Get employees with timesheets
- `PUT /api/admin/updateemployees/{id}` - Update employee details
- `DELETE /api/admin/deleteemployee/{id}` - Delete employee
- `GET /api/admin/getallusers` - Get all users
- `GET /api/admin/getuserbyid/{id}` - Get user by ID
- `POST /api/admin/createuser` - Create new user
- `PUT /api/admin/updateuser/{id}` - Update user details
- `DELETE /api/admin/deleteuser/{id}` - Delete user

### Reports & Dashboard
- `GET /api/admin/report/work-hours` - Get employee work hours report
- `GET /api/admin/dashboard` - Admin dashboard data
- `GET /api/admin/allEmployee/excelReport` - Export all employee timesheets to Excel

### Employee Features
- `GET /api/employeedashboard` - Get employee dashboard data
- `GET /api/employees/myprofile` - Get employee profile
- `PUT /api/employees/updateprofile` - Update employee profile
- `GET /api/timesheets/getmyrecords` - Get employee timesheet records
- `POST /api/timesheets/logworkhours` - Log employee work hours

---

## Technologies Used
- **Backend:** .NET Core 8 Web API
- **Database:** MS SQL Server (EF Core Code-First)
- **Authentication:** JWT-based authentication
- **Security:** Password hashing, email verification for password reset

---
## Contact
For questions, reach out at Moxshang@gmail.com or open an issue on GitHub.


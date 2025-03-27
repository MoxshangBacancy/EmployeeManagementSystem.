    using System.Text;
    using Employee_Management_System.Data;
    using Employee_Management_System.Data.Configurations;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.AspNetCore.Authentication.JwtBearer;
    using Microsoft.OpenApi.Models;
    using Employee_Management_System.Repository;
    using Employee_Management_System.Services;
using Employee_Management_System.Service;
using Microsoft.Extensions.Options;

    var builder = WebApplication.CreateBuilder(args);

    var jwtSettings = builder.Configuration.GetSection("Jwt");
    var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]);

    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero 
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

    options.AddPolicy("EmployeeOnly", policy => policy.RequireRole("Employee"));
});

builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddScoped<AuthService>();

builder.Services.AddScoped<EmailService>(); 

builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();

builder.Services.AddScoped<IEmployeeService, EmployeeService>();

builder.Services.AddScoped<IAdminRepository, AdminRepository>();

builder.Services.AddScoped<IAdminService, AdminService>();

builder.Services.AddScoped<ITimesheetRepository, TimesheetRepository>();

builder.Services.AddScoped<ITimesheetService, TimesheetService>();

builder.Services.AddScoped<ILeaveRepository, LeaveRepository>();

builder.Services.AddScoped<ILeaveService, LeaveService>();

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

builder.Services.AddScoped<IDepartmentService, DepartmentService>();


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles; // ✅ Fix: Removes `$id` and `$values`
        options.JsonSerializerOptions.WriteIndented = true;
    });

builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(c =>
    {
        c.SwaggerDoc("v1", new OpenApiInfo { Title = "Employee Management API", Version = "v1" });

        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            Scheme = "Bearer",
            BearerFormat = "JWT",
            In = ParameterLocation.Header,
            Description = "Enter 'Bearer {token}' to authenticate"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] {}
            }
        });
    });

    var app = builder.Build();

    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<AppDbContext>();

            Console.WriteLine("🔹 Checking database existence...");
            context.Database.Migrate(); 

            Console.WriteLine("🔹 Calling DbInitializer...");
            DbInitializer.Initialize(context);
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogError(ex, "❌ An error occurred while seeding the database.");
        }
    }

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseHttpsRedirection();
    app.UseAuthentication(); 
    app.UseAuthorization(); 

    app.MapControllers();
    app.Run();

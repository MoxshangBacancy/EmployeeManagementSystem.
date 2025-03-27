using Employee_Management_System.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Employee_Management_System.Data.Configurations
{
    public class TimesheetConfiguration : IEntityTypeConfiguration<Timesheet>
    {
        public void Configure(EntityTypeBuilder<Timesheet> builder)
        {
            builder.ToTable("Timesheets", t =>
            {
                t.HasCheckConstraint("CHK_Timesheet_TotalHours", "TotalHours >= 0");
            });

            builder.HasKey(t => t.TimesheetId);

            builder.Property(t => t.EmployeeId).IsRequired();
            builder.HasOne(t => t.Employee)
                   .WithMany(e => e.Timesheets)
                   .HasForeignKey(t => t.EmployeeId)
                   .OnDelete(DeleteBehavior.Cascade);

            builder.Property(t => t.Date)
                   .IsRequired()
                   .HasColumnType("DATE");

            builder.Property(t => t.StartTime)
                   .IsRequired()
                   .HasColumnType("TIME");

            builder.Property(t => t.EndTime)
                   .IsRequired()
                   .HasColumnType("TIME");

            builder.Property(t => t.TotalHours)
                   .IsRequired()
                   .HasDefaultValue(0);

            builder.Property(t => t.Description)
                   .HasMaxLength(255);

            builder.Property(t => t.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");
        }
    }
}

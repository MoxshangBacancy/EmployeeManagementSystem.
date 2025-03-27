using Employee_Management_System.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Employee_Management_System.Data.Configurations
{
    public class LeaveConfiguration : IEntityTypeConfiguration<Leave>
    {
            public void Configure(EntityTypeBuilder<Leave> builder)
            {
                builder.ToTable("Leaves", t =>
                {
                    t.HasCheckConstraint("CHK_Leave_Type", "LeaveType IN ('Sick Leave', 'Annual Leave', 'Casual Leave', 'Maternity Leave')");

                    t.HasCheckConstraint("CHK_Leave_Status", "Status IN ('Pending', 'Approved', 'Rejected')");
                });

                builder.HasKey(l => l.LeaveId);

                builder.Property(l => l.EmployeeId).IsRequired();
                builder.HasOne(l => l.Employee)
                       .WithMany(e => e.Leaves)
                       .HasForeignKey(l => l.EmployeeId)
                       .OnDelete(DeleteBehavior.Cascade); 

                builder.Property(l => l.StartDate)
                       .IsRequired()
                       .HasColumnType("DATE");

                builder.Property(l => l.EndDate)
                       .IsRequired()
                       .HasColumnType("DATE");

                builder.Property(l => l.LeaveType)
                       .IsRequired()
                       .HasMaxLength(50);

                builder.Property(l => l.Reason)
                       .HasMaxLength(255);

                builder.Property(l => l.Status)
                       .IsRequired()
                       .HasMaxLength(20);

                builder.Property(l => l.AppliedAt)
                       .HasDefaultValueSql("GETDATE()");
            }
        }
    }

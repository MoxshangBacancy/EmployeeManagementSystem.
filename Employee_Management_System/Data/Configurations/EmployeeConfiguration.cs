using Employee_Management_System.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Employee_Management_System.Data.Configurations
{
    public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
    {
        public void Configure(EntityTypeBuilder<Employee> builder)
        {
            builder.ToTable("Employees");

            builder.HasKey(e => e.EmployeeId);

            builder.Property(e => e.UserId).IsRequired();
            builder.HasOne(e => e.User)
                .WithOne()
                .HasForeignKey<Employee>(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(e => e.DateOfBirth)
                .IsRequired()
                .HasColumnType("DATE");

            builder.Property(e => e.Address)
                .HasMaxLength(255);

            builder.Property(e => e.TechStack)
                .HasMaxLength(100);

            builder.Property(e => e.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");

            builder.Property(e => e.UpdatedAt)
                   .HasDefaultValueSql("GETDATE()")
                   .ValueGeneratedOnAddOrUpdate();

            builder.Property(e => e.DepartmentId).IsRequired();
            builder.HasOne(e => e.Department)
                   .WithMany(d => d.Employees)
                   .HasForeignKey(e => e.DepartmentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

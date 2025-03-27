using Employee_Management_System.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace Employee_Management_System.Data.Configurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");

            builder.HasKey(d => d.DepartmentId);

            builder.Property(d => d.DepartmentName)
                   .IsRequired()
                   .HasMaxLength(100);
            builder.HasIndex(d => d.DepartmentName).IsUnique();

            builder.Property(d => d.CreatedAt)
                   .HasDefaultValueSql("GETDATE()");
        }
    }
}

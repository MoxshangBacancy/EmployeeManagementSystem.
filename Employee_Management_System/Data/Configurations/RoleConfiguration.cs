using Employee_Management_System.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Employee_Management_System.Data.Configurations
{
    public class RoleConfiguration: IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles", t =>
                    t.HasCheckConstraint("CHK_Role_Name", "RoleName IN ('Admin', 'Employee')")
            );
            builder.HasKey(r => r.RoleId);

            builder.Property(r => r.RoleName)
                .IsRequired()
                .HasMaxLength(50);
            builder.HasIndex(r => r.RoleName).IsUnique();
        }
    }
}

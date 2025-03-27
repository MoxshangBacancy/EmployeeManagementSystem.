using Employee_Management_System.Data.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users", t =>
        {
            t.HasCheckConstraint("CHK_User_Email", "Email LIKE '%@%.%'");

            t.HasCheckConstraint("CHK_User_Phone", "Phone NOT LIKE '%[^0-9]%'");
        });

        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.LastName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.Email)
               .IsRequired()
               .HasMaxLength(100)
               .HasColumnType("VARCHAR(100)");
        builder.HasIndex(u => u.Email).IsUnique();

        builder.Property(u => u.Phone)
               .HasMaxLength(15);
        builder.HasIndex(u => u.Phone).IsUnique(); 

        builder.Property(u => u.PasswordHash)
               .IsRequired()
               .HasMaxLength(255);

        builder.Property(u => u.CreatedAt)
               .HasDefaultValueSql("GETDATE()");

        builder.Property(u => u.UpdatedAt)
               .HasDefaultValueSql("GETDATE()")
               .ValueGeneratedOnAddOrUpdate();

        builder.Property(u => u.RoleId).IsRequired();
        builder.HasOne(u => u.Role)
               .WithMany(r => r.Users)
               .HasForeignKey(u => u.RoleId)
               .OnDelete(DeleteBehavior.Restrict);
    }
}

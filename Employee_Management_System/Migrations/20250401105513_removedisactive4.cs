using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class removedisactive4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive1",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive1",
                table: "Users");
        }
    }
}

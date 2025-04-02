using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Employee_Management_System.Migrations
{
    /// <inheritdoc />
    public partial class removedisactive2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive1",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "isDeleted",
                table: "Users");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive1",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isDeleted",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

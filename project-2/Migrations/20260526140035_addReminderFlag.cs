using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_2.Migrations
{
    /// <inheritdoc />
    public partial class addReminderFlag : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ReminderFlag",
                table: "Appointment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReminderFlag",
                table: "Appointment");
        }
    }
}

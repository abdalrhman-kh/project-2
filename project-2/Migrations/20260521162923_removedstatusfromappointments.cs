using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_2.Migrations
{
    /// <inheritdoc />
    public partial class removedstatusfromappointments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Status",
                table: "Appointment");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Status",
                table: "Appointment",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}

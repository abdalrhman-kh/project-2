using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_2.Migrations
{
    /// <inheritdoc />
    public partial class fixOTP : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "email",
                table: "OTPs",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "OTPs",
                newName: "Code");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Email",
                table: "OTPs",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "OTPs",
                newName: "code");
        }
    }
}

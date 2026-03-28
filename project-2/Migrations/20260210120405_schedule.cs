using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace project_2.Migrations
{
    /// <inheritdoc />
    public partial class schedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "Schedule",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<int>(
                name: "ScheduleId",
                table: "Appointment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Appointment_ScheduleId",
                table: "Appointment",
                column: "ScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointment_Schedule_ScheduleId",
                table: "Appointment",
                column: "ScheduleId",
                principalTable: "Schedule",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointment_Schedule_ScheduleId",
                table: "Appointment");

            migrationBuilder.DropIndex(
                name: "IX_Appointment_ScheduleId",
                table: "Appointment");

            migrationBuilder.DropColumn(
                name: "ScheduleId",
                table: "Appointment");

            migrationBuilder.AlterColumn<DateTime>(
                name: "Date",
                table: "Schedule",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }
    }
}

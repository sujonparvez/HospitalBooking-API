using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HospitalBooking.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "CreatedBy", "Email", "FullName", "IsDeleted", "PasswordHash", "PhoneNumber", "Role", "UpdatedAt", "UpdatedBy" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "System", "admin@hospital.com", "System Admin", false, "NeedsToBeHashedProperlyLater", "", 1, null, null });
        }
    }
}

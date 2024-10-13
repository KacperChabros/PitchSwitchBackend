using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PitchSwitchBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1efcb231-590c-4de1-ae61-964916dd03b5", null, "User", "USER" },
                    { "5972b934-2b29-4291-bd5c-3470d591f562", null, "Journalist", "JOURNALIST" },
                    { "cf4e7aa9-8322-46c9-b7ef-8d4d82a2f1c1", null, "Admin", "ADMIN" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1efcb231-590c-4de1-ae61-964916dd03b5");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5972b934-2b29-4291-bd5c-3470d591f562");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cf4e7aa9-8322-46c9-b7ef-8d4d82a2f1c1");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PitchSwitchBackend.Migrations
{
    /// <inheritdoc />
    public partial class Change_FK_AppUser_Club_Behaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Clubs_FavouriteClubId",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "11d845e0-412e-4bee-b200-0b68dccdbf99");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bb66689e-a38e-488a-b663-4878f80d0205");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "bcfd7720-f294-479a-a1a8-ca779586cab5");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "6c2aafe1-dd2e-415e-b452-371e9e200fda", null, "User", "USER" },
                    { "a6647646-67b9-4c38-9e0f-764406fc0801", null, "Journalist", "JOURNALIST" },
                    { "cc09f380-f194-4aff-9c43-183a07e73cd2", null, "Admin", "ADMIN" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Clubs_FavouriteClubId",
                table: "AspNetUsers",
                column: "FavouriteClubId",
                principalTable: "Clubs",
                principalColumn: "ClubId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Clubs_FavouriteClubId",
                table: "AspNetUsers");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "6c2aafe1-dd2e-415e-b452-371e9e200fda");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a6647646-67b9-4c38-9e0f-764406fc0801");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "cc09f380-f194-4aff-9c43-183a07e73cd2");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "11d845e0-412e-4bee-b200-0b68dccdbf99", null, "Admin", "ADMIN" },
                    { "bb66689e-a38e-488a-b663-4878f80d0205", null, "Journalist", "JOURNALIST" },
                    { "bcfd7720-f294-479a-a1a8-ca779586cab5", null, "User", "USER" }
                });

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Clubs_FavouriteClubId",
                table: "AspNetUsers",
                column: "FavouriteClubId",
                principalTable: "Clubs",
                principalColumn: "ClubId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitchSwitchBackend.Migrations
{
    /// <inheritdoc />
    public partial class Add_User_To_Transfer_Rumour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedByUserId",
                table: "TransferRumours",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRumours_CreatedByUserId",
                table: "TransferRumours",
                column: "CreatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_TransferRumours_AspNetUsers_CreatedByUserId",
                table: "TransferRumours",
                column: "CreatedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TransferRumours_AspNetUsers_CreatedByUserId",
                table: "TransferRumours");

            migrationBuilder.DropIndex(
                name: "IX_TransferRumours_CreatedByUserId",
                table: "TransferRumours");

            migrationBuilder.DropColumn(
                name: "CreatedByUserId",
                table: "TransferRumours");
        }
    }
}

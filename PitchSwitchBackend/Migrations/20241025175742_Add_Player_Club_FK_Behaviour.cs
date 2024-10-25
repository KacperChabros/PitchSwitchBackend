using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitchSwitchBackend.Migrations
{
    /// <inheritdoc />
    public partial class Add_Player_Club_FK_Behaviour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Clubs_FavouriteClubId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Clubs_FavouriteClubId",
                table: "AspNetUsers",
                column: "FavouriteClubId",
                principalTable: "Clubs",
                principalColumn: "ClubId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players",
                column: "ClubId",
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

            migrationBuilder.DropForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Clubs_FavouriteClubId",
                table: "AspNetUsers",
                column: "FavouriteClubId",
                principalTable: "Clubs",
                principalColumn: "ClubId");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Clubs_ClubId",
                table: "Players",
                column: "ClubId",
                principalTable: "Clubs",
                principalColumn: "ClubId");
        }
    }
}

using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitchSwitchBackend.Migrations
{
    /// <inheritdoc />
    public partial class Add_Transfer_Rumours : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TransferRumours",
                columns: table => new
                {
                    TransferRumourId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransferType = table.Column<string>(type: "nvarchar(30)", nullable: false),
                    RumouredFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ConfidenceLevel = table.Column<int>(type: "int", nullable: false),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    SellingClubId = table.Column<int>(type: "int", nullable: true),
                    BuyingClubId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferRumours", x => x.TransferRumourId);
                    table.ForeignKey(
                        name: "FK_TransferRumours_Clubs_BuyingClubId",
                        column: x => x.BuyingClubId,
                        principalTable: "Clubs",
                        principalColumn: "ClubId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRumours_Clubs_SellingClubId",
                        column: x => x.SellingClubId,
                        principalTable: "Clubs",
                        principalColumn: "ClubId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TransferRumours_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "PlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferRumours_BuyingClubId",
                table: "TransferRumours",
                column: "BuyingClubId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRumours_PlayerId",
                table: "TransferRumours",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TransferRumours_SellingClubId",
                table: "TransferRumours",
                column: "SellingClubId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferRumours");
        }
    }
}

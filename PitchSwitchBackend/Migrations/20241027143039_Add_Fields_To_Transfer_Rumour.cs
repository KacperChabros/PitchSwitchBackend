using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PitchSwitchBackend.Migrations
{
    /// <inheritdoc />
    public partial class Add_Fields_To_Transfer_Rumour : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsArchived",
                table: "TransferRumours",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsConfirmed",
                table: "TransferRumours",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsArchived",
                table: "TransferRumours");

            migrationBuilder.DropColumn(
                name: "IsConfirmed",
                table: "TransferRumours");
        }
    }
}

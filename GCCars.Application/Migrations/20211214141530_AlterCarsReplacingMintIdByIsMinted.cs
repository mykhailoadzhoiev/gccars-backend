using Microsoft.EntityFrameworkCore.Migrations;

namespace GCCars.Application.Migrations
{
    public partial class AlterCarsReplacingMintIdByIsMinted : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_MintId",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "MintId",
                table: "Cars");

            migrationBuilder.AddColumn<bool>(
                name: "IsMinted",
                table: "Cars",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsMinted",
                table: "Cars");

            migrationBuilder.AddColumn<int>(
                name: "MintId",
                table: "Cars",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cars_MintId",
                table: "Cars",
                column: "MintId",
                unique: true,
                filter: "[MintId] IS NOT NULL");
        }
    }
}

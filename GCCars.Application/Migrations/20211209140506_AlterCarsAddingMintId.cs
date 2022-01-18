using Microsoft.EntityFrameworkCore.Migrations;

namespace GCCars.Application.Migrations
{
    public partial class AlterCarsAddingMintId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_Name",
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

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Name",
                table: "Cars",
                column: "Name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_MintId",
                table: "Cars");

            migrationBuilder.DropIndex(
                name: "IX_Cars_Name",
                table: "Cars");

            migrationBuilder.DropColumn(
                name: "MintId",
                table: "Cars");

            migrationBuilder.CreateIndex(
                name: "IX_Cars_Name",
                table: "Cars",
                column: "Name",
                unique: true);
        }
    }
}

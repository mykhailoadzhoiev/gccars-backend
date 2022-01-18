using Microsoft.EntityFrameworkCore.Migrations;

namespace GCCars.Application.Migrations
{
    public partial class AlterCarsAddingUniqueIndexName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Cars_Name",
                table: "Cars",
                column: "Name",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Cars_Name",
                table: "Cars");
        }
    }
}

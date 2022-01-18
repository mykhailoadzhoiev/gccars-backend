using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GCCars.Application.Migrations
{
    public partial class AlterFightersAjustingForeignKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsTradable",
                table: "Cars",
                newName: "IsTradeable");

            migrationBuilder.CreateTable(
                name: "PvpBattles",
                columns: table => new
                {
                    PvpBattleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerId = table.Column<int>(type: "int", nullable: false),
                    Level = table.Column<int>(type: "int", nullable: true),
                    MaxFighters = table.Column<int>(type: "int", nullable: false),
                    BetAmount = table.Column<decimal>(type: "money", nullable: false),
                    StartedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    FinishedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PvpBattles", x => x.PvpBattleId);
                    table.ForeignKey(
                        name: "FK_PvpBattles_AspNetUsers_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Fighters",
                columns: table => new
                {
                    FighterId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PvpBattleId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fighters", x => x.FighterId);
                    table.ForeignKey(
                        name: "FK_Fighters_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "CarId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Fighters_PvpBattles_PvpBattleId",
                        column: x => x.PvpBattleId,
                        principalTable: "PvpBattles",
                        principalColumn: "PvpBattleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Fighters_CarId",
                table: "Fighters",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Fighters_PvpBattleId",
                table: "Fighters",
                column: "PvpBattleId");

            migrationBuilder.CreateIndex(
                name: "IX_PvpBattles_OwnerId",
                table: "PvpBattles",
                column: "OwnerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fighters");

            migrationBuilder.DropTable(
                name: "PvpBattles");

            migrationBuilder.RenameColumn(
                name: "IsTradeable",
                table: "Cars",
                newName: "IsTradable");
        }
    }
}

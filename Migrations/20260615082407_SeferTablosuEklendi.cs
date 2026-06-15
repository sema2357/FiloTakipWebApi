using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiloTakipWebApi.Migrations
{
    /// <inheritdoc />
    public partial class SeferTablosuEklendi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Seferler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    SoforId = table.Column<int>(type: "int", nullable: false),
                    CikisNoktasi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    VarisNoktasi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    BaslangicKm = table.Column<int>(type: "int", nullable: false),
                    BitisKm = table.Column<int>(type: "int", nullable: true),
                    Durum = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAd = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seferler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seferler_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Seferler_Soforler_SoforId",
                        column: x => x.SoforId,
                        principalTable: "Soforler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Seferler_AracId",
                table: "Seferler",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_Seferler_SoforId",
                table: "Seferler",
                column: "SoforId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Seferler");
        }
    }
}

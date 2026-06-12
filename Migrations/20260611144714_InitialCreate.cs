using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FiloTakipWebApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Araclar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Plaka = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Marka = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Model = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Yil = table.Column<int>(type: "int", nullable: false),
                    SasiNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    RuhsatNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GuncelKm = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Araclar", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Soforler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EhliyetSinifi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EhliyetGecerlilikTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TelNumarasi = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PerformansPuani = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    AktifAracId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soforler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Soforler_Araclar_AktifAracId",
                        column: x => x.AktifAracId,
                        principalTable: "Araclar",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_Plaka",
                table: "Araclar",
                column: "Plaka",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Soforler_AktifAracId",
                table: "Soforler",
                column: "AktifAracId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Soforler");

            migrationBuilder.DropTable(
                name: "Araclar");
        }
    }
}

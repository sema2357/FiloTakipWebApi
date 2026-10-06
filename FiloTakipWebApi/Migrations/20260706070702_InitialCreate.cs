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
                name: "BildirimSablonlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Konu = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    IcerikSablonu = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OlayTipi = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BildirimSablonlari", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MasrafKategorileri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MasrafKategorileri", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Soforler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Soyad = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TcNo = table.Column<string>(type: "nvarchar(11)", maxLength: 11, nullable: false),
                    Tel = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Eposta = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DogumTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EhliyetNo = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    EhliyetSinifi = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    EhliyetGecerlilikTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PerformansPuani = table.Column<decimal>(type: "decimal(5,2)", precision: 5, scale: 2, nullable: false),
                    SoforDurumu = table.Column<int>(type: "int", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Soforler", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Subeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ad = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Adres = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sehir = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Telefon = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subeler", x => x.Id);
                });

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
                    SasiNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    YakitTipi = table.Column<int>(type: "int", nullable: false),
                    AracDurumu = table.Column<int>(type: "int", nullable: false),
                    GuncelKm = table.Column<int>(type: "int", nullable: false),
                    GorselUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubeId = table.Column<int>(type: "int", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Araclar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Araclar_Subeler_SubeId",
                        column: x => x.SubeId,
                        principalTable: "Subeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Kullanicilar",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdSoyad = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    Eposta = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SifreHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Rol = table.Column<int>(type: "int", nullable: false),
                    SubeId = table.Column<int>(type: "int", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SonGirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kullanicilar", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Kullanicilar_Subeler_SubeId",
                        column: x => x.SubeId,
                        principalTable: "Subeler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AracSoforAtamalari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    SoforId = table.Column<int>(type: "int", nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AracSoforAtamalari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AracSoforAtamalari_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AracSoforAtamalari_Soforler_SoforId",
                        column: x => x.SoforId,
                        principalTable: "Soforler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BakimKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    BakimTipi = table.Column<int>(type: "int", nullable: false),
                    Baslik = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BakimTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TamamlanmaTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    KmOkumasi = table.Column<int>(type: "int", nullable: false),
                    SonrakiBakimKm = table.Column<int>(type: "int", nullable: true),
                    SonrakiBakimTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ServisAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MaliyetToplam = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: true),
                    ArizaKaydiMi = table.Column<bool>(type: "bit", nullable: false),
                    TamamlandiMi = table.Column<bool>(type: "bit", nullable: false),
                    Notlar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BakimKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BakimKayitlari_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Belgeler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    BelgeTipi = table.Column<int>(type: "int", nullable: false),
                    BelgeAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DosyaUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    GecerlilikTarihi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UyariBildirimMi = table.Column<bool>(type: "bit", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    UyariGunSayisi = table.Column<int>(type: "int", nullable: true),
                    YuklenmeTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Belgeler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Belgeler_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CezaIhlalKayitlari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SoforId = table.Column<int>(type: "int", nullable: false),
                    AracId = table.Column<int>(type: "int", nullable: true),
                    CezaTuru = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Tutar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: true),
                    OlayTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OdenmisMi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CezaIhlalKayitlari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CezaIhlalKayitlari_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_CezaIhlalKayitlari_Soforler_SoforId",
                        column: x => x.SoforId,
                        principalTable: "Soforler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Seferler",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    SoforId = table.Column<int>(type: "int", nullable: false),
                    BaslangicNoktasi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    VarisNoktasi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PlanlananBaslangic = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PlanlananBitis = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GercekBaslangic = table.Column<DateTime>(type: "datetime2", nullable: true),
                    GercekBitis = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PlanlananKm = table.Column<int>(type: "int", nullable: false),
                    GercekKm = table.Column<int>(type: "int", nullable: true),
                    YukBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YolcuBilgisi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Durumu = table.Column<int>(type: "int", nullable: false),
                    OnaylandiMi = table.Column<bool>(type: "bit", nullable: false),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    OnaylayanKullaniciId = table.Column<int>(type: "int", nullable: true),
                    SeferBelgesiUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IrsaliyeNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Notlar = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RedGerekce = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seferler", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seferler_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Seferler_Soforler_SoforId",
                        column: x => x.SoforId,
                        principalTable: "Soforler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SigortaPoliceleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    SigortaSirketi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PoliceNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    BaslangicTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    BitisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Prim = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    SigortaTuru = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DosyaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    AktifMi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SigortaPoliceleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SigortaPoliceleri_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "YakitGirisleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AracId = table.Column<int>(type: "int", nullable: false),
                    SoforId = table.Column<int>(type: "int", nullable: true),
                    GirisTarihi = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Litre = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(8,2)", precision: 8, scale: 2, nullable: false),
                    ToplamTutar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    KmOkumasi = table.Column<int>(type: "int", nullable: false),
                    PompaAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IstasyonAdi = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YakitKartiNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    YakitTipi = table.Column<int>(type: "int", nullable: false),
                    Litreper100Km = table.Column<decimal>(type: "decimal(6,2)", precision: 6, scale: 2, nullable: true),
                    AnormalTuketimMi = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_YakitGirisleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_YakitGirisleri_Araclar_AracId",
                        column: x => x.AracId,
                        principalTable: "Araclar",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_YakitGirisleri_Soforler_SoforId",
                        column: x => x.SoforId,
                        principalTable: "Soforler",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "BakimMasraflari",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BakimKaydiId = table.Column<int>(type: "int", nullable: false),
                    MasrafKategorisi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Aciklama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tutar = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BakimMasraflari", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BakimMasraflari_BakimKayitlari_BakimKaydiId",
                        column: x => x.BakimKaydiId,
                        principalTable: "BakimKayitlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BakimParcaDegisimleri",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BakimKaydiId = table.Column<int>(type: "int", nullable: false),
                    ParcaAdi = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Adet = table.Column<int>(type: "int", nullable: false),
                    BirimFiyat = table.Column<decimal>(type: "decimal(10,2)", precision: 10, scale: 2, nullable: false),
                    ToplamFiyat = table.Column<decimal>(type: "decimal(12,2)", precision: 12, scale: 2, nullable: false),
                    ParcaKodu = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BakimParcaDegisimleri", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BakimParcaDegisimleri_BakimKayitlari_BakimKaydiId",
                        column: x => x.BakimKaydiId,
                        principalTable: "BakimKayitlari",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_Plaka",
                table: "Araclar",
                column: "Plaka",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Araclar_SubeId",
                table: "Araclar",
                column: "SubeId");

            migrationBuilder.CreateIndex(
                name: "IX_AracSoforAtamalari_AracId",
                table: "AracSoforAtamalari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_AracSoforAtamalari_SoforId",
                table: "AracSoforAtamalari",
                column: "SoforId");

            migrationBuilder.CreateIndex(
                name: "IX_BakimKayitlari_AracId",
                table: "BakimKayitlari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_BakimMasraflari_BakimKaydiId",
                table: "BakimMasraflari",
                column: "BakimKaydiId");

            migrationBuilder.CreateIndex(
                name: "IX_BakimParcaDegisimleri_BakimKaydiId",
                table: "BakimParcaDegisimleri",
                column: "BakimKaydiId");

            migrationBuilder.CreateIndex(
                name: "IX_Belgeler_AracId",
                table: "Belgeler",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_CezaIhlalKayitlari_AracId",
                table: "CezaIhlalKayitlari",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_CezaIhlalKayitlari_SoforId",
                table: "CezaIhlalKayitlari",
                column: "SoforId");

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_Eposta",
                table: "Kullanicilar",
                column: "Eposta",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Kullanicilar_SubeId",
                table: "Kullanicilar",
                column: "SubeId");

            migrationBuilder.CreateIndex(
                name: "IX_Seferler_AracId",
                table: "Seferler",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_Seferler_SoforId",
                table: "Seferler",
                column: "SoforId");

            migrationBuilder.CreateIndex(
                name: "IX_SigortaPoliceleri_AracId",
                table: "SigortaPoliceleri",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_Soforler_TcNo",
                table: "Soforler",
                column: "TcNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_YakitGirisleri_AracId",
                table: "YakitGirisleri",
                column: "AracId");

            migrationBuilder.CreateIndex(
                name: "IX_YakitGirisleri_SoforId",
                table: "YakitGirisleri",
                column: "SoforId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AracSoforAtamalari");

            migrationBuilder.DropTable(
                name: "BakimMasraflari");

            migrationBuilder.DropTable(
                name: "BakimParcaDegisimleri");

            migrationBuilder.DropTable(
                name: "Belgeler");

            migrationBuilder.DropTable(
                name: "BildirimSablonlari");

            migrationBuilder.DropTable(
                name: "CezaIhlalKayitlari");

            migrationBuilder.DropTable(
                name: "Kullanicilar");

            migrationBuilder.DropTable(
                name: "MasrafKategorileri");

            migrationBuilder.DropTable(
                name: "Seferler");

            migrationBuilder.DropTable(
                name: "SigortaPoliceleri");

            migrationBuilder.DropTable(
                name: "YakitGirisleri");

            migrationBuilder.DropTable(
                name: "BakimKayitlari");

            migrationBuilder.DropTable(
                name: "Soforler");

            migrationBuilder.DropTable(
                name: "Araclar");

            migrationBuilder.DropTable(
                name: "Subeler");
        }
    }
}

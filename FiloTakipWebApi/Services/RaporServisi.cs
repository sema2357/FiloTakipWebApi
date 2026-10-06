using Dapper;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Models.Enums;
using System.Data;

namespace FiloTakipWebApi.Services
{
    public interface IRaporServisi
    {
        Task<IEnumerable<AracMaliyetRaporDto>> AracMaliyetRaporuAsync(int? aracId, DateTime baslangic, DateTime bitis);
        Task<IEnumerable<SoforPerformansRaporDto>> SoforPerformansRaporuAsync(DateTime baslangic, DateTime bitis);
        Task<FiloGenelOzetDto> FiloGenelOzetiniGetirAsync();
    }

    public class RaporServisi : IRaporServisi
    {
        private readonly DapperContext _context;

        public RaporServisi(DapperContext context) => _context = context;

        public async Task<IEnumerable<AracMaliyetRaporDto>> AracMaliyetRaporuAsync(
            int? aracId, DateTime baslangic, DateTime bitis)
        {
            var query = @"
                SELECT 
                    y.AracId, 
                    a.Plaka, 
                    a.Marka, 
                    SUM(y.ToplamTutar) as ToplamYakit,
                    ISNULL((SELECT SUM(b.MaliyetToplam) FROM BakimKayitlari b WHERE b.AracId = y.AracId AND b.BakimTarihi BETWEEN @Baslangic AND @Bitis AND b.MaliyetToplam IS NOT NULL), 0) as BakimMaliyeti,
                    (MAX(y.KmOkumasi) - MIN(y.KmOkumasi)) as KatEdilenKm
                FROM YakitGirisleri y
                INNER JOIN Araclar a ON y.AracId = a.Id
                WHERE y.GirisTarihi BETWEEN @Baslangic AND @Bitis
                  AND (@AracId IS NULL OR y.AracId = @AracId)
                GROUP BY y.AracId, a.Plaka, a.Marka";

            using var connection = _context.CreateConnection();
            var dynamicResult = await connection.QueryAsync<dynamic>(query, new { Baslangic = baslangic, Bitis = bitis, AracId = aracId });

            return dynamicResult.Select(r => new AracMaliyetRaporDto(
                (string)r.Plaka, 
                (string)r.Marka, 
                (decimal)r.ToplamYakit, 
                (decimal)r.BakimMaliyeti,
                (decimal)r.ToplamYakit + (decimal)r.BakimMaliyeti,
                (int)Math.Round((decimal)r.KatEdilenKm)
            ));
        }

        public async Task<IEnumerable<SoforPerformansRaporDto>> SoforPerformansRaporuAsync(
            DateTime baslangic, DateTime bitis)
        {
            var query = @"
                SELECT 
                    s.SoforId,
                    sof.Ad,
                    sof.Soyad,
                    sof.PerformansPuani,
                    SUM(CASE WHEN s.Durumu = @TamamlandiDurum THEN 1 ELSE 0 END) as TamamlananSefer,
                    ISNULL((SELECT COUNT(1) FROM CezaIhlalKayitlari c WHERE c.SoforId = s.SoforId AND c.OlayTarihi BETWEEN @Baslangic AND @Bitis), 0) as CezaSayisi,
                    ISNULL((SELECT SUM(c.Tutar) FROM CezaIhlalKayitlari c WHERE c.SoforId = s.SoforId AND c.OlayTarihi BETWEEN @Baslangic AND @Bitis), 0) as CezaTutari
                FROM Seferler s
                INNER JOIN Soforler sof ON s.SoforId = sof.Id
                WHERE s.PlanlananBaslangic BETWEEN @Baslangic AND @Bitis
                GROUP BY s.SoforId, sof.Ad, sof.Soyad, sof.PerformansPuani";

            using var connection = _context.CreateConnection();
            var dynamicResult = await connection.QueryAsync<dynamic>(query, new { 
                Baslangic = baslangic, 
                Bitis = bitis,
                TamamlandiDurum = (int)SeferDurumu.Tamamlandi
            });

            return dynamicResult.Select(r => new SoforPerformansRaporDto(
                $"{r.Ad} {r.Soyad}",
                (int)r.TamamlananSefer,
                (int)r.PerformansPuani,
                (int)r.CezaSayisi,
                (decimal)r.CezaTutari
            ));
        }

        public async Task<FiloGenelOzetDto> FiloGenelOzetiniGetirAsync()
        {
            var query = @"
                DECLARE @Bugun DATE = CAST(GETDATE() AS DATE);
                DECLARE @AyBaslangic DATE = DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0);

                SELECT 
                    (SELECT COUNT(1) FROM Araclar WHERE AktifMi = 1) as ToplamArac,
                    (SELECT COUNT(1) FROM Araclar WHERE AktifMi = 1 AND AracDurumu = @AktifAracDurumu) as AktifArac,
                    (SELECT COUNT(1) FROM Araclar WHERE AktifMi = 1 AND AracDurumu = @BakimdaAracDurumu) as BakimdakiArac,
                    (SELECT COUNT(1) FROM Soforler WHERE AktifMi = 1) as ToplamSofor,
                    (SELECT COUNT(1) FROM Seferler WHERE CAST(PlanlananBaslangic AS DATE) = @Bugun) as BugunkuSefer,
                    (SELECT COUNT(1) FROM Seferler WHERE Durumu = @DevamEdenSeferDurumu) as DevamEdenSefer,
                    ISNULL((SELECT SUM(ToplamTutar) FROM YakitGirisleri WHERE GirisTarihi >= @AyBaslangic), 0) as AylikYakitMaliyeti,
                    ISNULL((SELECT SUM(MaliyetToplam) FROM BakimKayitlari WHERE BakimTarihi >= @AyBaslangic AND MaliyetToplam IS NOT NULL), 0) as AylikBakimMaliyeti;
            ";

            using var connection = _context.CreateConnection();
            var r = await connection.QuerySingleAsync<dynamic>(query, new {
                AktifAracDurumu = (int)AracDurumu.Aktif,
                BakimdaAracDurumu = (int)AracDurumu.Bakimda,
                DevamEdenSeferDurumu = (int)SeferDurumu.DevamEdiyor
            });

            return new FiloGenelOzetDto(
                (int)r.ToplamArac,
                (int)r.AktifArac,
                (int)r.BakimdakiArac,
                (int)r.ToplamSofor,
                (int)r.BugunkuSefer,
                (int)r.DevamEdenSefer,
                (decimal)r.AylikYakitMaliyeti,
                (decimal)r.AylikBakimMaliyeti
            );
        }
    }
}

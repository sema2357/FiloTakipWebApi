using System.Data;
using Dapper;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;

namespace FiloTakipWebApi.Services
{
    public interface IYakitServisi
    {
        Task<IEnumerable<YakitGirisiDto>> AracYakitlariniGetirAsync(int aracId, DateTime? baslangic, DateTime? bitis);
        Task<YakitGirisiDto> YakitGirisiEkleAsync(YakitGirisiOlusturDto dto);
        Task<IEnumerable<YakitTuketimAnalizDto>> TuketimAnaliziAsync(int? aracId, DateTime? baslangic, DateTime? bitis);
        Task<IEnumerable<YakitGirisiDto>> AnormalTuketimleriGetirAsync();
    }

    public class YakitServisi : IYakitServisi
    {
        private readonly DapperContext _context;
        private const decimal AnormalEsikYuzdesi = 0.25m;

        public YakitServisi(DapperContext context) => _context = context;

        public async Task<IEnumerable<YakitGirisiDto>> AracYakitlariniGetirAsync(int aracId, DateTime? baslangic, DateTime? bitis)
        {
            var sql = @"
                SELECT 
                    y.Id, a.Plaka as AracPlaka, 
                    CASE WHEN s.Ad IS NOT NULL THEN s.Ad + ' ' + s.Soyad ELSE NULL END as SoforAdSoyad,
                    y.GirisTarihi, y.Litre, y.BirimFiyat, y.ToplamTutar,
                    y.KmOkumasi, y.IstasyonAdi, y.Litreper100Km, y.AnormalTuketimMi
                FROM YakitGirisleri y
                INNER JOIN Araclar a ON y.AracId = a.Id
                LEFT JOIN Soforler s ON y.SoforId = s.Id
                WHERE y.AracId = @AracId
                {0}
                {1}
                ORDER BY y.GirisTarihi DESC";

            var filterStart = baslangic.HasValue ? "AND y.GirisTarihi >= @Baslangic" : "";
            var filterEnd = bitis.HasValue ? "AND y.GirisTarihi <= @Bitis" : "";

            sql = string.Format(sql, filterStart, filterEnd);

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<YakitGirisiDto>(sql, new { AracId = aracId, Baslangic = baslangic, Bitis = bitis });
        }

        public async Task<YakitGirisiDto> YakitGirisiEkleAsync(YakitGirisiOlusturDto dto)
        {
            var toplam = dto.Litre * dto.BirimFiyat;
            decimal? litrePer100Km = null;
            bool anormal = false;

            using var connection = _context.CreateConnection();
            
            var oncekiGirisSql = "SELECT TOP 1 * FROM YakitGirisleri WHERE AracId = @AracId AND KmOkumasi < @KmOkumasi ORDER BY KmOkumasi DESC";
            var oncekiGiris = await connection.QueryFirstOrDefaultAsync(oncekiGirisSql, new { dto.AracId, dto.KmOkumasi });

            if (oncekiGiris != null)
            {
                var kmFarki = dto.KmOkumasi - (decimal)oncekiGiris.KmOkumasi;
                if (kmFarki > 0)
                {
                    litrePer100Km = dto.Litre / kmFarki * 100;
                    var ortalamaSql = "SELECT AVG(CAST(Litreper100Km AS decimal(18,4))) FROM YakitGirisleri WHERE AracId = @AracId AND Litreper100Km IS NOT NULL";
                    var ortalama = await connection.ExecuteScalarAsync<decimal?>(ortalamaSql, new { dto.AracId }) ?? litrePer100Km;

                    anormal = litrePer100Km > ortalama * (1 + AnormalEsikYuzdesi) ||
                              litrePer100Km < ortalama * (1 - AnormalEsikYuzdesi);
                }
            }

            var insertSql = @"
                INSERT INTO YakitGirisleri (AracId, SoforId, GirisTarihi, Litre, BirimFiyat, ToplamTutar, KmOkumasi, PompaAdi, IstasyonAdi, YakitKartiNo, YakitTipi, Litreper100Km, AnormalTuketimMi)
                VALUES (@AracId, @SoforId, @GirisTarihi, @Litre, @BirimFiyat, @ToplamTutar, @KmOkumasi, @PompaAdi, @IstasyonAdi, @YakitKartiNo, @YakitTipi, @Litreper100Km, @AnormalTuketimMi);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var newId = await connection.QuerySingleAsync<int>(insertSql, new
            {
                dto.AracId, dto.SoforId, dto.GirisTarihi, dto.Litre, dto.BirimFiyat, ToplamTutar = toplam,
                dto.KmOkumasi, dto.PompaAdi, dto.IstasyonAdi, dto.YakitKartiNo, dto.YakitTipi, Litreper100Km = litrePer100Km, AnormalTuketimMi = anormal
            });

            var aracUpdateSql = "UPDATE Araclar SET GuncelKm = @KmOkumasi WHERE Id = @AracId AND GuncelKm < @KmOkumasi";
            await connection.ExecuteAsync(aracUpdateSql, new { dto.KmOkumasi, dto.AracId });

            var getSql = @"
                SELECT 
                    y.Id, a.Plaka as AracPlaka, 
                    CASE WHEN s.Ad IS NOT NULL THEN s.Ad + ' ' + s.Soyad ELSE NULL END as SoforAdSoyad,
                    y.GirisTarihi, y.Litre, y.BirimFiyat, y.ToplamTutar,
                    y.KmOkumasi, y.IstasyonAdi, y.Litreper100Km, y.AnormalTuketimMi
                FROM YakitGirisleri y
                INNER JOIN Araclar a ON y.AracId = a.Id
                LEFT JOIN Soforler s ON y.SoforId = s.Id
                WHERE y.Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<YakitGirisiDto>(getSql, new { Id = newId });
        }

        public async Task<IEnumerable<YakitTuketimAnalizDto>> TuketimAnaliziAsync(int? aracId, DateTime? baslangic, DateTime? bitis)
        {
            var sql = @"
                SELECT 
                    a.Plaka as Plaka, 
                    ROUND(AVG(CAST(y.Litreper100Km as float)), 2) as OrtalamaTuketim, 
                    SUM(y.Litre) as ToplamLitre, 
                    SUM(y.ToplamTutar) as ToplamMaliyet, 
                    SUM(CASE WHEN y.AnormalTuketimMi = 1 THEN 1 ELSE 0 END) as AnormalTuketimSayisi
                FROM YakitGirisleri y
                INNER JOIN Araclar a ON y.AracId = a.Id
                WHERE y.Litreper100Km IS NOT NULL
                {0}
                {1}
                {2}
                GROUP BY y.AracId, a.Plaka";

            var filterArac = aracId.HasValue ? "AND y.AracId = @AracId" : "";
            var filterStart = baslangic.HasValue ? "AND y.GirisTarihi >= @Baslangic" : "";
            var filterEnd = bitis.HasValue ? "AND y.GirisTarihi <= @Bitis" : "";

            sql = string.Format(sql, filterArac, filterStart, filterEnd);

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<YakitTuketimAnalizDto>(sql, new { AracId = aracId, Baslangic = baslangic, Bitis = bitis });
        }

        public async Task<IEnumerable<YakitGirisiDto>> AnormalTuketimleriGetirAsync()
        {
            var sql = @"
                SELECT 
                    y.Id, a.Plaka as AracPlaka, 
                    CASE WHEN s.Ad IS NOT NULL THEN s.Ad + ' ' + s.Soyad ELSE NULL END as SoforAdSoyad,
                    y.GirisTarihi, y.Litre, y.BirimFiyat, y.ToplamTutar,
                    y.KmOkumasi, y.IstasyonAdi, y.Litreper100Km, y.AnormalTuketimMi
                FROM YakitGirisleri y
                INNER JOIN Araclar a ON y.AracId = a.Id
                LEFT JOIN Soforler s ON y.SoforId = s.Id
                WHERE y.AnormalTuketimMi = 1
                ORDER BY y.GirisTarihi DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<YakitGirisiDto>(sql);
        }
    }

    public interface IBakimServisi
    {
        Task<IEnumerable<BakimKaydiDto>> AracBakimlariGetirAsync(int aracId);
        Task<IEnumerable<BakimKaydiDto>> YaklasanBakimlariGetirAsync(int gunSayisi = 30);
        Task<BakimKaydiDto> BakimKaydiEkleAsync(BakimKaydiOlusturDto dto);
        Task<bool> BakimTamamlaAsync(int id, DateTime tamamlanmaTarihi);
    }

    public class BakimServisi : IBakimServisi
    {
        private readonly DapperContext _context;

        public BakimServisi(DapperContext context) => _context = context;

        public async Task<IEnumerable<BakimKaydiDto>> AracBakimlariGetirAsync(int aracId)
        {
            var sql = @"
                SELECT 
                    b.Id, a.Plaka as AracPlaka, b.BakimTipi, b.Baslik,
                    b.BakimTarihi, b.TamamlanmaTarihi, b.KmOkumasi,
                    b.SonrakiBakimKm, b.ServisAdi, b.MaliyetToplam, b.TamamlandiMi
                FROM BakimKayitlari b
                INNER JOIN Araclar a ON b.AracId = a.Id
                WHERE b.AracId = @AracId
                ORDER BY b.BakimTarihi DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<BakimKaydiDto>(sql, new { AracId = aracId });
        }

        public async Task<IEnumerable<BakimKaydiDto>> YaklasanBakimlariGetirAsync(int gunSayisi = 30)
        {
            var esikTarih = DateTime.Today.AddDays(gunSayisi);
            var sql = @"
                SELECT 
                    b.Id, a.Plaka as AracPlaka, b.BakimTipi, b.Baslik,
                    b.BakimTarihi, b.TamamlanmaTarihi, b.KmOkumasi,
                    b.SonrakiBakimKm, b.ServisAdi, b.MaliyetToplam, b.TamamlandiMi
                FROM BakimKayitlari b
                INNER JOIN Araclar a ON b.AracId = a.Id
                WHERE b.TamamlandiMi = 0 
                  AND b.SonrakiBakimTarihi IS NOT NULL 
                  AND b.SonrakiBakimTarihi <= @EsikTarih
                ORDER BY b.SonrakiBakimTarihi";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<BakimKaydiDto>(sql, new { EsikTarih = esikTarih });
        }

        public async Task<BakimKaydiDto> BakimKaydiEkleAsync(BakimKaydiOlusturDto dto)
        {
            var insertSql = @"
                INSERT INTO BakimKayitlari (AracId, BakimTipi, Baslik, Aciklama, BakimTarihi, KmOkumasi, SonrakiBakimKm, SonrakiBakimTarihi, ServisAdi, ArizaKaydiMi, Notlar)
                VALUES (@AracId, @BakimTipi, @Baslik, @Aciklama, @BakimTarihi, @KmOkumasi, @SonrakiBakimKm, @SonrakiBakimTarihi, @ServisAdi, @ArizaKaydiMi, @Notlar);
                SELECT CAST(SCOPE_IDENTITY() as int);";

            using var connection = _context.CreateConnection();
            var newId = await connection.QuerySingleAsync<int>(insertSql, new
            {
                dto.AracId, dto.BakimTipi, dto.Baslik, dto.Aciklama, dto.BakimTarihi, dto.KmOkumasi, 
                dto.SonrakiBakimKm, dto.SonrakiBakimTarihi, dto.ServisAdi, dto.ArizaKaydiMi, dto.Notlar
            });

            var getSql = @"
                SELECT 
                    b.Id, a.Plaka as AracPlaka, b.BakimTipi, b.Baslik,
                    b.BakimTarihi, b.TamamlanmaTarihi, b.KmOkumasi,
                    b.SonrakiBakimKm, b.ServisAdi, b.MaliyetToplam, b.TamamlandiMi
                FROM BakimKayitlari b
                INNER JOIN Araclar a ON b.AracId = a.Id
                WHERE b.Id = @Id";

            return await connection.QueryFirstOrDefaultAsync<BakimKaydiDto>(getSql, new { Id = newId });
        }

        public async Task<bool> BakimTamamlaAsync(int id, DateTime tamamlanmaTarihi)
        {
            var sql = "UPDATE BakimKayitlari SET TamamlandiMi = 1, TamamlanmaTarihi = @TamamlanmaTarihi WHERE Id = @Id";
            
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, TamamlanmaTarihi = tamamlanmaTarihi });
            return affectedRows > 0;
        }
    }
}

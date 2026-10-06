using System.Data;
using Dapper;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;

namespace FiloTakipWebApi.Services
{
    public interface IBelgeServisi
    {
        Task<IEnumerable<BelgeDto>> TumBelgeleriGetirAsync();
        Task<BelgeDto?> BelgeGetirAsync(int id);
        Task<IEnumerable<BelgeDto>> AracaAitBelgeleriGetirAsync(int aracId);
        Task<BelgeDto?> BelgeEkleAsync(BelgeOlusturDto dto);
        Task<bool> BelgeSilAsync(int id);
    }

    public class BelgeServisi : IBelgeServisi
    {
        private readonly DapperContext _context;

        public BelgeServisi(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BelgeDto>> TumBelgeleriGetirAsync()
        {
            var sql = @"
                SELECT 
                    b.Id, a.Plaka as AracPlaka, b.BelgeTipi, b.BelgeAdi, b.DosyaUrl as DosyaYolu, 
                    b.GecerlilikTarihi, b.UyariBildirimMi, b.UyariGunSayisi, b.CreatedAt as YuklenmeTarihi
                FROM Belgeler b
                INNER JOIN Araclar a ON b.AracId = a.Id
                WHERE b.AktifMi = 1
                ORDER BY b.CreatedAt DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<BelgeDto>(sql);
        }

        public async Task<BelgeDto?> BelgeGetirAsync(int id)
        {
            var sql = @"
                SELECT 
                    b.Id, a.Plaka as AracPlaka, b.BelgeTipi, b.BelgeAdi, b.DosyaUrl as DosyaYolu, 
                    b.GecerlilikTarihi, b.UyariBildirimMi, b.UyariGunSayisi, b.CreatedAt as YuklenmeTarihi
                FROM Belgeler b
                INNER JOIN Araclar a ON b.AracId = a.Id
                WHERE b.Id = @Id AND b.AktifMi = 1";

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<BelgeDto>(sql, new { Id = id });
        }

        public async Task<IEnumerable<BelgeDto>> AracaAitBelgeleriGetirAsync(int aracId)
        {
            var sql = @"
                SELECT 
                    b.Id, a.Plaka as AracPlaka, b.BelgeTipi, b.BelgeAdi, b.DosyaUrl as DosyaYolu, 
                    b.GecerlilikTarihi, b.UyariBildirimMi, b.UyariGunSayisi, b.CreatedAt as YuklenmeTarihi
                FROM Belgeler b
                INNER JOIN Araclar a ON b.AracId = a.Id
                WHERE b.AktifMi = 1 AND b.AracId = @AracId
                ORDER BY b.GecerlilikTarihi DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<BelgeDto>(sql, new { AracId = aracId });
        }

        public async Task<BelgeDto?> BelgeEkleAsync(BelgeOlusturDto dto)
        {
            var sql = @"
                INSERT INTO Belgeler (AracId, BelgeTipi, BelgeAdi, DosyaUrl, GecerlilikTarihi, UyariBildirimMi, UyariGunSayisi, YuklenmeTarihi, CreatedAt, AktifMi)
                OUTPUT INSERTED.Id
                VALUES (@AracId, @BelgeTipi, @BelgeAdi, @DosyaYolu, @GecerlilikTarihi, @UyariBildirimMi, @UyariGunSayisi, @YuklenmeTarihi, @CreatedAt, 1)";
            
            var parameters = new {
                dto.AracId, dto.BelgeTipi, dto.BelgeAdi, dto.DosyaYolu, dto.GecerlilikTarihi, dto.UyariBildirimMi, dto.UyariGunSayisi, 
                YuklenmeTarihi = DateTime.Now, CreatedAt = DateTime.Now
            };

            using var connection = _context.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return await BelgeGetirAsync(id);
        }

        public async Task<bool> BelgeSilAsync(int id)
        {
            var sql = "UPDATE Belgeler SET AktifMi = 0, CreatedAt = @CreatedAt WHERE Id = @Id AND AktifMi = 1";
            
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CreatedAt = DateTime.Now });
            return affectedRows > 0;
        }
    }
}
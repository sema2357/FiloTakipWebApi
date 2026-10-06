using System.Data;
using Dapper;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;

namespace FiloTakipWebApi.Services
{
    public interface ISigortaServisi
    {
        Task<IEnumerable<SigortaPolicesiDto>> TumSigortalariGetirAsync();
        Task<SigortaPolicesiDto?> SigortaGetirAsync(int id);
        Task<IEnumerable<SigortaPolicesiDto>> AracinSigortalariGetirAsync(int aracId);
        Task<SigortaPolicesiDto> SigortaOlusturAsync(SigortaPolicesiOlusturDto dto);
        Task<bool> SigortaSilAsync(int id);
    }

    public class SigortaServisi : ISigortaServisi
    {
        private readonly DapperContext _context;

        public SigortaServisi(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SigortaPolicesiDto>> TumSigortalariGetirAsync()
        {
            var sql = @"
                SELECT 
                    s.Id, a.Plaka as AracPlaka, s.SigortaSirketi, s.PoliceNo, s.BaslangicTarihi, 
                    s.BitisTarihi, s.Prim, s.SigortaTuru, s.AktifMi
                FROM SigortaPoliceleri s
                INNER JOIN Araclar a ON s.AracId = a.Id
                WHERE s.AktifMi = 1
                ORDER BY s.BitisTarihi DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SigortaPolicesiDto>(sql);
        }

        public async Task<SigortaPolicesiDto?> SigortaGetirAsync(int id)
        {
            var sql = @"
                SELECT 
                    s.Id, a.Plaka as AracPlaka, s.SigortaSirketi, s.PoliceNo, s.BaslangicTarihi, 
                    s.BitisTarihi, s.Prim, s.SigortaTuru, s.AktifMi
                FROM SigortaPoliceleri s
                INNER JOIN Araclar a ON s.AracId = a.Id
                WHERE s.Id = @Id AND s.AktifMi = 1";

            using var connection = _context.CreateConnection();
            return await connection.QueryFirstOrDefaultAsync<SigortaPolicesiDto>(sql, new { Id = id });
        }

        public async Task<IEnumerable<SigortaPolicesiDto>> AracinSigortalariGetirAsync(int aracId)
        {
            var sql = @"
                SELECT 
                    s.Id, a.Plaka as AracPlaka, s.SigortaSirketi, s.PoliceNo, s.BaslangicTarihi, 
                    s.BitisTarihi, s.Prim, s.SigortaTuru, s.AktifMi
                FROM SigortaPoliceleri s
                INNER JOIN Araclar a ON s.AracId = a.Id
                WHERE s.AracId = @AracId AND s.AktifMi = 1
                ORDER BY s.BitisTarihi DESC";

            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SigortaPolicesiDto>(sql, new { AracId = aracId });
        }

        public async Task<SigortaPolicesiDto> SigortaOlusturAsync(SigortaPolicesiOlusturDto dto)
        {
            var insertSql = @"
                INSERT INTO SigortaPoliceleri (AracId, SigortaSirketi, PoliceNo, BaslangicTarihi, BitisTarihi, Prim, SigortaTuru, AktifMi)
                VALUES (@AracId, @SigortaSirketi, @PoliceNo, @BaslangicTarihi, @BitisTarihi, @Prim, @SigortaTuru, 1);
                SELECT CAST(SCOPE_IDENTITY() as int);";
            
            using var connection = _context.CreateConnection();
            var newId = await connection.QuerySingleAsync<int>(insertSql, new
            {
                dto.AracId,
                dto.SigortaSirketi,
                dto.PoliceNo,
                dto.BaslangicTarihi,
                dto.BitisTarihi,
                dto.Prim,
                dto.SigortaTuru
            });

            return await SigortaGetirAsync(newId);
        }

        public async Task<bool> SigortaSilAsync(int id)
        {
            var sql = "UPDATE SigortaPoliceleri SET AktifMi = 0, CreatedAt = @CreatedAt WHERE Id = @Id AND AktifMi = 1";
            
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id, CreatedAt = DateTime.Now });
            return affectedRows > 0;
        }
    }
}
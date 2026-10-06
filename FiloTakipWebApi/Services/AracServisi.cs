using Dapper;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Models.Entities;
using FiloTakipWebApi.Models.Enums;
using System.Text;

namespace FiloTakipWebApi.Services
{
    public interface IAracServisi
    {
        Task<SayfalamaliSonucDto<AracDto>> TumAraclariGetirAsync(int sayfa, int boyut, string? plaka, AracDurumu? durum);
        Task<AracDto?> AracGetirAsync(int id);
        Task<AracDto> AracOlusturAsync(AracOlusturDto dto);
        Task<AracDto?> AracGuncelleAsync(int id, AracGuncelleDto dto);
        Task<bool> AracSilAsync(int id);
        Task<bool> KilometreGuncelleAsync(int id, KmGuncelleDto dto);
        Task<bool> AracGorselGuncelleAsync(int id, string gorselUrl);
        Task<IEnumerable<AracDto>> GecerlilikSuresiDolanAraclariGetirAsync(int gunSayisi = 30);
    }

    public class AracServisi : IAracServisi
    {
        private readonly DapperContext _context;

        public AracServisi(DapperContext context)
        {
            _context = context;
        }

        public async Task<SayfalamaliSonucDto<AracDto>> TumAraclariGetirAsync(
            int sayfa, int boyut, string? plaka, AracDurumu? durum)
        {
            using var connection = _context.CreateConnection();
            var sqlBuilder = new StringBuilder("FROM Araclar a LEFT JOIN Subeler s ON a.SubeId = s.Id WHERE a.AktifMi = 1");
            var parameters = new DynamicParameters();

            if (!string.IsNullOrEmpty(plaka))
            {
                sqlBuilder.Append(" AND a.Plaka LIKE @Plaka");
                parameters.Add("Plaka", $"%{plaka}%");
            }

            if (durum.HasValue)
            {
                sqlBuilder.Append(" AND a.AracDurumu = @Durum");
                parameters.Add("Durum", (int)durum.Value);
            }

            var countSql = $"SELECT COUNT(1) {sqlBuilder}";
            var toplam = await connection.ExecuteScalarAsync<int>(countSql, parameters);

            var selectSql = $@"
                SELECT a.*, s.* 
                {sqlBuilder} 
                ORDER BY a.Plaka 
                OFFSET @Offset ROWS FETCH NEXT @Boyut ROWS ONLY";
            
            parameters.Add("Offset", (sayfa - 1) * boyut);
            parameters.Add("Boyut", boyut);

            var araclari = await connection.QueryAsync<Arac, Sube, Arac>(selectSql, (a, s) => {
                a.Sube = s;
                return a;
            }, parameters, splitOn: "Id");

            var veriler = araclari.Select(a => AracaDonustur(a)).ToList();
            return new SayfalamaliSonucDto<AracDto>(veriler, toplam, sayfa, boyut);
        }

        public async Task<AracDto?> AracGetirAsync(int id)
        {
            var sql = "SELECT a.*, s.* FROM Araclar a LEFT JOIN Subeler s ON a.SubeId = s.Id WHERE a.Id = @Id AND a.AktifMi = 1";
            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Arac, Sube, Arac>(sql, (a, s) => {
                a.Sube = s;
                return a;
            }, new { Id = id }, splitOn: "Id");
            
            var arac = result.FirstOrDefault();
            return arac is null ? null : AracaDonustur(arac);
        }

        public async Task<AracDto> AracOlusturAsync(AracOlusturDto dto)
        {
            var sql = @"
                INSERT INTO Araclar (Plaka, Marka, Model, Yil, SasiNo, YakitTipi, AracDurumu, GuncelKm, SubeId, AktifMi, CreatedAt)
                OUTPUT INSERTED.Id
                VALUES (@Plaka, @Marka, @Model, @Yil, @SasiNo, @YakitTipi, 0, 0, @SubeId, 1, @CreatedAt)";
            
            var parameters = new {
                Plaka = dto.Plaka.ToUpperInvariant(), dto.Marka, dto.Model, dto.Yil, dto.SasiNo, dto.YakitTipi, dto.SubeId, CreatedAt = DateTime.Now
            };

            using var connection = _context.CreateConnection();
            var id = await connection.ExecuteScalarAsync<int>(sql, parameters);
            return await AracGetirAsync(id) ?? throw new Exception("Araç oluşturulamadı");
        }

        public async Task<AracDto?> AracGuncelleAsync(int id, AracGuncelleDto dto)
        {
            var sql = @"
                UPDATE Araclar SET 
                    Marka = @Marka, Model = @Model, Yil = @Yil, SasiNo = @SasiNo, 
                    YakitTipi = @YakitTipi, AracDurumu = @AracDurumu, GuncelKm = @GuncelKm, 
                    SubeId = @SubeId, UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND AktifMi = 1";

            var parameters = new {
                dto.Marka, dto.Model, dto.Yil, dto.SasiNo, dto.YakitTipi, dto.AracDurumu, dto.GuncelKm, dto.SubeId, UpdatedAt = DateTime.Now, Id = id
            };

            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, parameters);
            if (affected == 0) return null;
            
            return await AracGetirAsync(id);
        }

        public async Task<bool> AracSilAsync(int id)
        {
            var sql = "UPDATE Araclar SET AktifMi = 0 WHERE Id = @Id AND AktifMi = 1";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }

        public async Task<bool> KilometreGuncelleAsync(int id, KmGuncelleDto dto)
        {
            var sql = "UPDATE Araclar SET GuncelKm = @YeniKm, UpdatedAt = @UpdatedAt WHERE Id = @Id AND AktifMi = 1 AND GuncelKm <= @YeniKm";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { dto.YeniKm, UpdatedAt = DateTime.Now, Id = id });
            return affected > 0;
        }

        public async Task<bool> AracGorselGuncelleAsync(int id, string gorselUrl)
        {
            var sql = "UPDATE Araclar SET GorselUrl = @GorselUrl, UpdatedAt = @UpdatedAt WHERE Id = @Id AND AktifMi = 1";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { GorselUrl = gorselUrl, UpdatedAt = DateTime.Now, Id = id });
            return affected > 0;
        }

        public async Task<IEnumerable<AracDto>> GecerlilikSuresiDolanAraclariGetirAsync(int gunSayisi = 30)
        {
            var esikTarih = DateTime.Today.AddDays(gunSayisi);
            var sql = @"
                SELECT DISTINCT a.*, s.* 
                FROM Araclar a
                LEFT JOIN Subeler s ON a.SubeId = s.Id
                LEFT JOIN Belgeler b ON a.Id = b.AracId
                LEFT JOIN SigortaPoliceleri sp ON a.Id = sp.AracId
                WHERE a.AktifMi = 1 AND (
                    (b.GecerlilikTarihi <= @EsikTarih) OR
                    (sp.AktifMi = 1 AND sp.BitisTarihi <= @EsikTarih)
                )";

            using var connection = _context.CreateConnection();
            var result = await connection.QueryAsync<Arac, Sube, Arac>(sql, (a, s) => {
                a.Sube = s;
                return a;
            }, new { EsikTarih = esikTarih }, splitOn: "Id");

            return result.Select(a => AracaDonustur(a)).ToList();
        }

        private static AracDto AracaDonustur(Arac a) => new(
            a.Id, a.Plaka, a.Marka, a.Model, a.Yil, a.SasiNo,
            a.YakitTipi, a.AracDurumu, a.GuncelKm, a.GorselUrl,
            a.Sube?.Ad, a.CreatedAt);
    }
}

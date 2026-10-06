using Dapper;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Models.Entities;
using FiloTakipWebApi.Models.Enums;
using System.Data;

namespace FiloTakipWebApi.Services
{
    public interface ISeferServisi
    {
        Task<IEnumerable<SeferDto>> TumSeferleriGetirAsync();
        Task<SeferDto?> SeferGetirAsync(int id);
        Task<SeferDto> SeferOlusturAsync(SeferOlusturDto dto);
        Task<bool> SeferOnaylaAsync(SeferOnayDto dto);
        Task<bool> SeferSilAsync(int id);
    }

    public class SeferServisi : ISeferServisi
    {
        private readonly DapperContext _context;

        public SeferServisi(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SeferDto>> TumSeferleriGetirAsync()
        {
            var query = @"
                SELECT s.*, a.*, sof.*
                FROM Seferler s
                INNER JOIN Araclar a ON s.AracId = a.Id
                INNER JOIN Soforler sof ON s.SoforId = sof.Id
                WHERE s.AktifMi = 1
                ORDER BY s.PlanlananBaslangic DESC";

            using var connection = _context.CreateConnection();
            var seferler = await connection.QueryAsync<Sefer, Arac, Sofor, SeferDto>(
                query,
                (sefer, arac, sofor) =>
                {
                    return new SeferDto(
                        sefer.Id,
                        arac.Plaka,
                        $"{sofor.Ad} {sofor.Soyad}",
                        sefer.BaslangicNoktasi,
                        sefer.VarisNoktasi,
                        sefer.PlanlananBaslangic,
                        sefer.PlanlananBitis,
                        sefer.GercekBaslangic,
                        sefer.GercekBitis,
                        sefer.PlanlananKm,
                        sefer.GercekKm,
                        sefer.Durumu,
                        sefer.OnaylandiMi,
                        sefer.IrsaliyeNo
                    );
                },
                splitOn: "Id,Id"
            );
            return seferler;
        }

        public async Task<SeferDto?> SeferGetirAsync(int id)
        {
            var query = @"
                SELECT s.*, a.*, sof.*
                FROM Seferler s
                INNER JOIN Araclar a ON s.AracId = a.Id
                INNER JOIN Soforler sof ON s.SoforId = sof.Id
                WHERE s.Id = @Id AND s.AktifMi = 1";

            using var connection = _context.CreateConnection();
            var seferler = await connection.QueryAsync<Sefer, Arac, Sofor, SeferDto>(
                query,
                (sefer, arac, sofor) =>
                {
                    return new SeferDto(
                        sefer.Id,
                        arac.Plaka,
                        $"{sofor.Ad} {sofor.Soyad}",
                        sefer.BaslangicNoktasi,
                        sefer.VarisNoktasi,
                        sefer.PlanlananBaslangic,
                        sefer.PlanlananBitis,
                        sefer.GercekBaslangic,
                        sefer.GercekBitis,
                        sefer.PlanlananKm,
                        sefer.GercekKm,
                        sefer.Durumu,
                        sefer.OnaylandiMi,
                        sefer.IrsaliyeNo
                    );
                },
                new { Id = id },
                splitOn: "Id,Id"
            );

            return seferler.FirstOrDefault();
        }

        public async Task<SeferDto> SeferOlusturAsync(SeferOlusturDto dto)
        {
            using var connection = _context.CreateConnection();

            var aracQuery = "SELECT * FROM Araclar WHERE Id = @Id AND AktifMi = 1";
            var arac = await connection.QueryFirstOrDefaultAsync<Arac>(aracQuery, new { Id = dto.AracId });

            if (arac is null)
                throw new Exception("Araç bulunamadı.");

            if (arac.AracDurumu == AracDurumu.Bakimda)
                throw new Exception("Araç bakımda olduğu için sefere çıkamaz.");

            var soforQuery = "SELECT * FROM Soforler WHERE Id = @Id AND AktifMi = 1";
            var sofor = await connection.QueryFirstOrDefaultAsync<Sofor>(soforQuery, new { Id = dto.SoforId });

            if (sofor is null)
                throw new Exception("Şoför bulunamadı.");

            var aracMusaitDegilQuery = @"
                SELECT COUNT(1) FROM Seferler
                WHERE AktifMi = 1 AND AracId = @AracId
                AND @PlanlananBaslangic < PlanlananBitis
                AND @PlanlananBitis > PlanlananBaslangic";
            
            var aracCakismaCount = await connection.ExecuteScalarAsync<int>(aracMusaitDegilQuery, new { 
                dto.AracId, 
                dto.PlanlananBaslangic, 
                dto.PlanlananBitis 
            });

            if (aracCakismaCount > 0)
                throw new Exception("Bu araç belirtilen tarihler arasında başka bir sefere atanmıştır.");

            var soforMusaitDegilQuery = @"
                SELECT COUNT(1) FROM Seferler
                WHERE AktifMi = 1 AND SoforId = @SoforId
                AND @PlanlananBaslangic < PlanlananBitis
                AND @PlanlananBitis > PlanlananBaslangic";

            var soforCakismaCount = await connection.ExecuteScalarAsync<int>(soforMusaitDegilQuery, new {
                dto.SoforId,
                dto.PlanlananBaslangic,
                dto.PlanlananBitis
            });

            if (soforCakismaCount > 0)
                throw new Exception("Şoför belirtilen saatlerde başka bir seferdedir.");

            if (dto.PlanlananBaslangic >= dto.PlanlananBitis)
                throw new Exception("Başlangıç tarihi bitiş tarihinden büyük olamaz.");

            if (dto.PlanlananKm <= 0)
                throw new Exception("Planlanan kilometre 0'dan büyük olmalıdır.");

            if (dto.PlanlananBaslangic < DateTime.Now.AddMinutes(-5))
                throw new Exception("Geçmiş tarih için sefer oluşturulamaz.");

            var insertQuery = @"
                INSERT INTO Seferler (
                    AracId, SoforId, BaslangicNoktasi, VarisNoktasi, PlanlananBaslangic, PlanlananBitis, PlanlananKm,
                    YukBilgisi, YolcuBilgisi, IrsaliyeNo, Notlar, Durumu, OnaylandiMi, AktifMi
                )
                VALUES (
                    @AracId, @SoforId, @BaslangicNoktasi, @VarisNoktasi, @PlanlananBaslangic, @PlanlananBitis, @PlanlananKm,
                    @YukBilgisi, @YolcuBilgisi, @IrsaliyeNo, @Notlar, @Durumu, @OnaylandiMi, @AktifMi
                );
                SELECT CAST(SCOPE_IDENTITY() as int);";

            var seferParameters = new {
                dto.AracId,
                dto.SoforId,
                dto.BaslangicNoktasi,
                dto.VarisNoktasi,
                dto.PlanlananBaslangic,
                dto.PlanlananBitis,
                dto.PlanlananKm,
                dto.YukBilgisi,
                dto.YolcuBilgisi,
                dto.IrsaliyeNo,
                dto.Notlar,
                Durumu = (int)SeferDurumu.Planlandi,
                OnaylandiMi = false,
                AktifMi = true
            };

            var seferId = await connection.QuerySingleAsync<int>(insertQuery, seferParameters);

            return new SeferDto(
                seferId,
                arac.Plaka,
                $"{sofor.Ad} {sofor.Soyad}",
                dto.BaslangicNoktasi,
                dto.VarisNoktasi,
                dto.PlanlananBaslangic,
                dto.PlanlananBitis,
                null,
                null,
                dto.PlanlananKm,
                null,
                SeferDurumu.Planlandi,
                false,
                dto.IrsaliyeNo
            );
        }

        public async Task<bool> SeferOnaylaAsync(SeferOnayDto dto)
        {
            var updateQuery = @"
                UPDATE Seferler
                SET OnaylandiMi = @Onayla,
                    RedGerekce = CASE WHEN @Onayla = 1 THEN NULL ELSE @RedGerekce END
                WHERE Id = @SeferId AND AktifMi = 1";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(updateQuery, new {
                dto.Onayla,
                RedGerekce = dto.Onayla ? null : dto.RedGerekce,
                dto.SeferId
            });

            return affectedRows > 0;
        }

        public async Task<bool> SeferSilAsync(int id)
        {
            var updateQuery = @"
                UPDATE Seferler
                SET AktifMi = 0
                WHERE Id = @Id AND AktifMi = 1";

            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(updateQuery, new { Id = id });

            return affectedRows > 0;
        }
    }
}
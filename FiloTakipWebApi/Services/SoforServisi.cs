using Dapper;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Models.Entities;

namespace FiloTakipWebApi.Services
{
    public interface ISoforServisi
    {
        Task<IEnumerable<SoforDto>> TumSoforleriGetirAsync();
        Task<SoforDto?> SoforGetirAsync(int id);
        Task<SoforDto> SoforOlusturAsync(SoforOlusturDto dto);
        Task<SoforDto?> SoforGuncelleAsync(int id, SoforGuncelleDto dto);
        Task<bool> SoforSilAsync(int id);
    }

    public class SoforServisi : ISoforServisi
    {
        private readonly DapperContext _context;

        public SoforServisi(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SoforDto>> TumSoforleriGetirAsync()
        {
            var sql = "SELECT * FROM Soforler WHERE AktifMi = 1 ORDER BY Ad, Soyad";
            using var connection = _context.CreateConnection();
            var soforler = await connection.QueryAsync<Sofor>(sql);
            return soforler.Select(s => SoforeDonustur(s));
        }

        public async Task<SoforDto?> SoforGetirAsync(int id)
        {
            var sql = "SELECT * FROM Soforler WHERE Id = @Id AND AktifMi = 1";
            using var connection = _context.CreateConnection();
            var sofor = await connection.QuerySingleOrDefaultAsync<Sofor>(sql, new { Id = id });
            
            return sofor is null ? null : SoforeDonustur(sofor);
        }

        public async Task<SoforDto> SoforOlusturAsync(SoforOlusturDto dto)
        {
            var sql = @"
                INSERT INTO Soforler (Ad, Soyad, TcNo, Tel, Eposta, DogumTarihi, EhliyetNo, EhliyetSinifi, EhliyetGecerlilikTarihi, PerformansPuani, SoforDurumu, AktifMi, CreatedAt)
                OUTPUT INSERTED.*
                VALUES (@Ad, @Soyad, @TcNo, @Tel, @Eposta, @DogumTarihi, @EhliyetNo, @EhliyetSinifi, @EhliyetGecerlilikTarihi, 100, 0, 1, @CreatedAt)";
            
            var parameters = new {
                dto.Ad, dto.Soyad, dto.TcNo, dto.Tel, dto.Eposta, dto.DogumTarihi, dto.EhliyetNo, dto.EhliyetSinifi, dto.EhliyetGecerlilikTarihi, CreatedAt = DateTime.Now
            };

            using var connection = _context.CreateConnection();
            var sofor = await connection.QuerySingleAsync<Sofor>(sql, parameters);
            return SoforeDonustur(sofor);
        }

        public async Task<SoforDto?> SoforGuncelleAsync(int id, SoforGuncelleDto dto)
        {
            var sql = @"
                UPDATE Soforler SET 
                    Ad = @Ad, Soyad = @Soyad, TcNo = @TcNo, Tel = @Tel, Eposta = @Eposta, 
                    DogumTarihi = @DogumTarihi, EhliyetNo = @EhliyetNo, EhliyetSinifi = @EhliyetSinifi, 
                    EhliyetGecerlilikTarihi = @EhliyetGecerlilikTarihi
                OUTPUT INSERTED.*
                WHERE Id = @Id AND AktifMi = 1";

            var parameters = new {
                dto.Ad, dto.Soyad, dto.TcNo, dto.Tel, dto.Eposta, dto.DogumTarihi, dto.EhliyetNo, dto.EhliyetSinifi, dto.EhliyetGecerlilikTarihi, Id = id
            };

            using var connection = _context.CreateConnection();
            var sofor = await connection.QuerySingleOrDefaultAsync<Sofor>(sql, parameters);
            return sofor is null ? null : SoforeDonustur(sofor);
        }

        public async Task<bool> SoforSilAsync(int id)
        {
            var sql = "UPDATE Soforler SET AktifMi = 0 WHERE Id = @Id AND AktifMi = 1";
            using var connection = _context.CreateConnection();
            var affectedRows = await connection.ExecuteAsync(sql, new { Id = id });
            return affectedRows > 0;
        }

        private static SoforDto SoforeDonustur(Sofor sofor)
            => new(
                sofor.Id,
                sofor.Ad,
                sofor.Soyad,
                sofor.TcNo ?? "",
                sofor.Tel,
                sofor.Eposta,
                sofor.EhliyetNo ?? "",
                sofor.EhliyetSinifi,
                sofor.EhliyetGecerlilikTarihi,
                sofor.PerformansPuani,
                sofor.AktifMi
            );
    }
}
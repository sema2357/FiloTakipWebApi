using System.Data;
using Dapper;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;

namespace FiloTakipWebApi.Services
{
    public interface ISubeServisi
    {
        Task<IEnumerable<SubeDto>> TumSubeleriGetirAsync();
        Task<SubeDto> SubeEkleAsync(SubeOlusturDto dto);
        Task<bool> SubeSilAsync(int id);
    }

    public class SubeServisi : ISubeServisi
    {
        private readonly DapperContext _context;
        public SubeServisi(DapperContext context) => _context = context;

        public async Task<IEnumerable<SubeDto>> TumSubeleriGetirAsync()
        {
            var sql = "SELECT Id, Ad, Adres, Sehir, Telefon, AktifMi FROM Subeler WHERE AktifMi = 1";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<SubeDto>(sql);
        }

        public async Task<SubeDto> SubeEkleAsync(SubeOlusturDto dto)
        {
            var sql = @"
                INSERT INTO Subeler (Ad, Adres, Sehir, Telefon, AktifMi) 
                OUTPUT INSERTED.Id, INSERTED.Ad, INSERTED.Adres, INSERTED.Sehir, INSERTED.Telefon, INSERTED.AktifMi
                VALUES (@Ad, @Adres, @Sehir, @Telefon, 1)";
            using var connection = _context.CreateConnection();
            return await connection.QuerySingleAsync<SubeDto>(sql, dto);
        }

        public async Task<bool> SubeSilAsync(int id)
        {
            var sql = "UPDATE Subeler SET AktifMi = 0 WHERE Id = @Id AND AktifMi = 1";
            using var connection = _context.CreateConnection();
            var affected = await connection.ExecuteAsync(sql, new { Id = id });
            return affected > 0;
        }
    }
}

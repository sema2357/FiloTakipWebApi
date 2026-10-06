using Dapper;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Models.Entities;
using FiloTakipWebApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FiloTakipWebApi.Services
{
    public interface IKimlikServisi
    {
        Task<GirisYanitiDto?> GirisYapAsync(GirisIstegiDto dto);
        Task<KullaniciOlusturDto> KullaniciOlusturAsync(KullaniciOlusturDto dto);
        Task<IEnumerable<KullaniciDto>> TumKullanicilariGetirAsync();
    }

    public class KimlikServisi : IKimlikServisi
    {
        private readonly DapperContext _context;
        private readonly IConfiguration _config;

        public KimlikServisi(DapperContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        public async Task<GirisYanitiDto?> GirisYapAsync(GirisIstegiDto dto)
        {
            var sql = "SELECT * FROM Kullanicilar WHERE Eposta = @Eposta AND AktifMi = 1";
            using var connection = _context.CreateConnection();
            var kullanici = await connection.QuerySingleOrDefaultAsync<Kullanici>(sql, new { dto.Eposta });

            if (kullanici is null || !BCrypt.Net.BCrypt.Verify(dto.Sifre, kullanici.SifreHash))
                return null;

            var updateSql = "UPDATE Kullanicilar SET SonGirisTarihi = @SonGirisTarihi WHERE Id = @Id AND AktifMi = 1";
            await connection.ExecuteAsync(updateSql, new { SonGirisTarihi = DateTime.Now, kullanici.Id });

            var token = TokenOlustur(kullanici);
            return new GirisYanitiDto(token, kullanici.AdSoyad, kullanici.Rol, kullanici.SubeId);
        }

        public async Task<KullaniciOlusturDto> KullaniciOlusturAsync(KullaniciOlusturDto dto)
        {
            var sql = @"
                INSERT INTO Kullanicilar (AdSoyad, Eposta, SifreHash, Rol, SubeId, AktifMi, CreatedAt)
                VALUES (@AdSoyad, @Eposta, @SifreHash, @Rol, @SubeId, 1, @CreatedAt)";

            var parameters = new {
                dto.AdSoyad, dto.Eposta, SifreHash = BCrypt.Net.BCrypt.HashPassword(dto.Sifre), dto.Rol, dto.SubeId, CreatedAt = DateTime.Now
            };

            using var connection = _context.CreateConnection();
            await connection.ExecuteAsync(sql, parameters);
            return dto;
        }

        public async Task<IEnumerable<KullaniciDto>> TumKullanicilariGetirAsync()
        {
            var sql = "SELECT Id, AdSoyad, Eposta, Rol, SubeId FROM Kullanicilar WHERE AktifMi = 1";
            using var connection = _context.CreateConnection();
            return await connection.QueryAsync<KullaniciDto>(sql);
        }

        private string TokenOlustur(Kullanici kullanici)
        {
            var anahtar = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Anahtar"]
                    ?? throw new InvalidOperationException("Jwt:Anahtar ayarı tanımlı değil.")));

            var bildirimler = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new Claim(ClaimTypes.Email, kullanici.Eposta),
                new Claim(ClaimTypes.Name, kullanici.AdSoyad),
                new Claim(ClaimTypes.Role, kullanici.Rol.ToString()),
                new Claim("SubeId", kullanici.SubeId?.ToString() ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Yayinci"] ?? "FiloTakip",
                audience: _config["Jwt:Izleyici"] ?? "FiloTakip",
                claims: bildirimler,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: new SigningCredentials(anahtar, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

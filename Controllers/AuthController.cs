using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FiloTakipWebApi.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("kayit")]
        public async Task<IActionResult> Kayit([FromBody] KullaniciKayitDto dto)
        {
            var varMi = await _context.Kullanicilar.AnyAsync(k => k.KullaniciAdi == dto.KullaniciAdi);
            if (varMi) return BadRequest("Bu kullanıcı adı zaten alınmış.");

            var yeniKullanici = new Kullanici
            {
                KullaniciAdi = dto.KullaniciAdi,
                SifreHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(dto.Sifre)),
                Rol = dto.Rol

            };

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();
            return Ok("Kullanıcı başarıyla oluşturuldu.");
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] KullaniciGirisDto dto)
        {
            var sifreHash = Convert.ToBase64String(Encoding.UTF8.GetBytes(dto.Sifre));
            var kullanici = await _context.Kullanicilar
                .FirstOrDefaultAsync(k => k.KullaniciAdi == dto.KullaniciAdi && k.SifreHash == sifreHash);

            if (kullanici == null) return Unauthorized("Hatalı kullanıcı adı veya şifre.");

            //token
            var jwtSettings = _configuration.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]));

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, kullanici.Id.ToString()),
                new Claim(ClaimTypes.Name, kullanici.KullaniciAdi),
                new Claim(ClaimTypes.Role, kullanici.Rol)
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(3),
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { Token = tokenString, Mesaj = "Giriş Başarılı" });



        }


    }

}
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KimlikController : ControllerBase
    {
        private readonly IKimlikServisi _kimlikServisi;
        public KimlikController(IKimlikServisi kimlikServisi) => _kimlikServisi = kimlikServisi;

        /// <summary>Sisteme giriş yap, JWT token al</summary>
        [HttpPost("giris")]
        [AllowAnonymous]
        public async Task<ActionResult<GirisYanitiDto>> GirisYap([FromBody] GirisIstegiDto dto)
        {
            var yanit = await _kimlikServisi.GirisYapAsync(dto);
            if (yanit is null) return Unauthorized(new HataYanitiDto("Geçersiz e-posta veya şifre."));
            return Ok(yanit);
        }

        /// <summary>Sistemdeki kullanıcıları getirir (sadece Admin ve Yonetici)</summary>
        [HttpGet("kullanicilar")]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<ActionResult<IEnumerable<KullaniciDto>>> KullanicilariGetir()
        {
            return Ok(await _kimlikServisi.TumKullanicilariGetirAsync());
        }

        /// <summary>Yeni kullanıcı oluştur (sadece Admin)</summary>
        [HttpPost("kullanici-olustur")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> KullaniciOlustur([FromBody] KullaniciOlusturDto dto)
        {
            await _kimlikServisi.KullaniciOlusturAsync(dto);
            return Created("", new { mesaj = "Kullanıcı başarıyla oluşturuldu." });
        }
    }
}

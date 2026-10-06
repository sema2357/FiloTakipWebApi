using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Models.Enums;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace FiloTakipWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class AraclarController : ControllerBase
    {
        private readonly IAracServisi _aracServisi;
        public AraclarController(IAracServisi aracServisi) => _aracServisi = aracServisi;

        /// <summary>Tüm araçları listele (sayfalama)</summary>
        [HttpGet]
        public async Task<ActionResult<SayfalamaliSonucDto<AracDto>>> TumAraclar(
            [FromQuery] int sayfa = 1,
            [FromQuery] int boyut = 20,
            [FromQuery] string? plaka = null,
            [FromQuery] AracDurumu? durum = null)
        {
            var sonuc = await _aracServisi.TumAraclariGetirAsync(sayfa, boyut, plaka, durum);
            return Ok(sonuc);
        }

        /// <summary>Tekil araç getir</summary>
        [HttpGet("{id:int}")]
        public async Task<ActionResult<AracDto>> AracGetir(int id)
        {
            var arac = await _aracServisi.AracGetirAsync(id);
            return arac is null ? NotFound(new HataYanitiDto($"ID={id} araç bulunamadı.")) : Ok(arac);
        }

        /// <summary>Yeni araç ekle</summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<ActionResult<AracDto>> AracEkle([FromBody] AracOlusturDto dto)
        {
            var arac = await _aracServisi.AracOlusturAsync(dto);
            return CreatedAtAction(nameof(AracGetir), new { id = arac.Id }, arac);
        }

        /// <summary>Araç bilgilerini güncelle</summary>
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Admin,Yonetici")]
        public async Task<ActionResult<AracDto>> AracGuncelle(int id, [FromBody] AracGuncelleDto dto)
        {
            var arac = await _aracServisi.AracGuncelleAsync(id, dto);
            return arac is null ? NotFound() : Ok(arac);
        }

        /// <summary>Araç kilometre güncelle</summary>
        [HttpPatch("{id:int}/kilometre")]
        public async Task<ActionResult> KilometreGuncelle(int id, [FromBody]  KmGuncelleDto dto)
        {
            var basarili = await _aracServisi.KilometreGuncelleAsync(id, dto);
            return basarili ? NoContent() : BadRequest(new HataYanitiDto("Geçersiz kilometre değeri."));
        }

        /// <summary>Araç görsel güncelle</summary>
        [HttpPatch("{id:int}/gorsel")]
        public async Task<ActionResult> GorselGuncelle(int id, [FromBody] string gorselUrl)
        {
            var basarili = await _aracServisi.AracGorselGuncelleAsync(id, gorselUrl);
            return basarili ? NoContent() : NotFound();
        }

        /// <summary>Geçerlilik süresi dolan araçlar (sigorta/muayene/ruhsat)</summary>
        [HttpGet("gecerlilik-uyarilari")]
        public async Task<ActionResult<IEnumerable<AracDto>>> GecerlilikUyarilari([FromQuery] int gunSayisi = 30)
        {
            var liste = await _aracServisi.GecerlilikSuresiDolanAraclariGetirAsync(gunSayisi);
            return Ok(liste);
        }

        /// <summary>Araç pasif yap (soft delete)</summary>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> AracSil(int id)
        {
            var basarili = await _aracServisi.AracSilAsync(id);
            return basarili ? NoContent() : NotFound();
        }
    }
}

using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BakimController : ControllerBase
    {
        private readonly IBakimServisi _bakimServisi;
        public BakimController(IBakimServisi bakimServisi) => _bakimServisi = bakimServisi;

        /// <summary>Araç bakım kayıtları</summary>
        [HttpGet("arac/{aracId:int}")]
        public async Task<ActionResult<IEnumerable<BakimKaydiDto>>> AracBakimlari(int aracId)
        {
            var liste = await _bakimServisi.AracBakimlariGetirAsync(aracId);
            return Ok(liste);
        }

        /// <summary>Yaklaşan bakımlar hatırlatıcısı</summary>
        [HttpGet("yaklasan-bakimlar")]
        public async Task<ActionResult<IEnumerable<BakimKaydiDto>>> YaklasanBakimlar([FromQuery] int gunSayisi = 30)
        {
            var liste = await _bakimServisi.YaklasanBakimlariGetirAsync(gunSayisi);
            return Ok(liste);
        }

        /// <summary>Yeni bakım/arıza kaydı ekle</summary>
        [HttpPost]
        public async Task<ActionResult<BakimKaydiDto>> BakimKaydiEkle([FromBody] BakimKaydiOlusturDto dto)
        {
            var kayit = await _bakimServisi.BakimKaydiEkleAsync(dto);
            return Created("", kayit);
        }

        /// <summary>Bakım tamamlandı olarak işaretle</summary>
        [HttpPatch("{id:int}/tamamla")]
        public async Task<ActionResult> BakimTamamla(int id, [FromBody] DateTime tamamlanmaTarihi)
        {
            var basarili = await _bakimServisi.BakimTamamlaAsync(id, tamamlanmaTarihi);
            return basarili ? NoContent() : NotFound();
        }
    }
}

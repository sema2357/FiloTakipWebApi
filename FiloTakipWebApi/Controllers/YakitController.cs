using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class YakitController : ControllerBase
    {
        private readonly IYakitServisi _yakitServisi;
        public YakitController(IYakitServisi yakitServisi) => _yakitServisi = yakitServisi;

        /// <summary>Araç yakıt geçmişi</summary>
        [HttpGet("arac/{aracId:int}")]
        public async Task<ActionResult<IEnumerable<YakitGirisiDto>>> AracYakitlari(
            int aracId,
            [FromQuery] DateTime? baslangic,
            [FromQuery] DateTime? bitis)
        {
            var liste = await _yakitServisi.AracYakitlariniGetirAsync(aracId, baslangic, bitis);
            return Ok(liste);
        }

        /// <summary>Yakıt girişi ekle</summary>
        [HttpPost]
        public async Task<ActionResult<YakitGirisiDto>> YakitGirisiEkle([FromBody] YakitGirisiOlusturDto dto)
        {
            var giris = await _yakitServisi.YakitGirisiEkleAsync(dto);
            return CreatedAtAction(null, giris);
        }

        /// <summary>Yakıt tüketim analizi</summary>
        [HttpGet("tuketim-analizi")]
        public async Task<ActionResult<IEnumerable<YakitTuketimAnalizDto>>> TuketimAnalizi(
            [FromQuery] int? aracId,
            [FromQuery] DateTime? baslangic,
            [FromQuery] DateTime? bitis)
        {
            var analiz = await _yakitServisi.TuketimAnaliziAsync(aracId, baslangic, bitis);
            return Ok(analiz);
        }

        /// <summary>Anormal tüketim kayıtları</summary>
        [HttpGet("anormal-tuketimler")]
        public async Task<ActionResult<IEnumerable<YakitGirisiDto>>> AnormalTuketimler()
        {
            var liste = await _yakitServisi.AnormalTuketimleriGetirAsync();
            return Ok(liste);
        }
    }
}

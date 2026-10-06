using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class RaporlarController : ControllerBase
    {
        private readonly IRaporServisi _raporServisi;
        public RaporlarController(IRaporServisi raporServisi) => _raporServisi = raporServisi;

        /// <summary>Araç maliyet raporu</summary>
        [HttpGet("arac-maliyeti")]
        public async Task<ActionResult<IEnumerable<AracMaliyetRaporDto>>> AracMaliyetRaporu(
            [FromQuery] int? aracId,
            [FromQuery] DateTime baslangic,
            [FromQuery] DateTime bitis)
        {
            var rapor = await _raporServisi.AracMaliyetRaporuAsync(aracId, baslangic, bitis);
            return Ok(rapor);
        }

        /// <summary>Şoför performans raporu</summary>
        [HttpGet("sofor-performans")]
        public async Task<ActionResult<IEnumerable<SoforPerformansRaporDto>>> SoforPerformansRaporu(
            [FromQuery] DateTime baslangic,
            [FromQuery] DateTime bitis)
        {
            var rapor = await _raporServisi.SoforPerformansRaporuAsync(baslangic, bitis);
            return Ok(rapor);
        }

        /// <summary>Filo genel özeti (dashboard)</summary>
        [HttpGet("filo-ozeti")]
        public async Task<ActionResult<FiloGenelOzetDto>> FiloOzeti()
        {
            var ozet = await _raporServisi.FiloGenelOzetiniGetirAsync();
            return Ok(ozet);
        }
    }

}

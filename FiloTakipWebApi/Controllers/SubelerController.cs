using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class SubelerController : ControllerBase
    {
        private readonly ISubeServisi _subeServisi;
        public SubelerController(ISubeServisi subeServisi) => _subeServisi = subeServisi;

        [HttpGet]
        public async Task<ActionResult<IEnumerable<SubeDto>>> TumSubeler()
        {
            return Ok(await _subeServisi.TumSubeleriGetirAsync());
        }

        [HttpPost]
        public async Task<ActionResult<SubeDto>> SubeEkle([FromBody] SubeOlusturDto dto)
        {
            var result = await _subeServisi.SubeEkleAsync(dto);
            return Ok(result);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> SubeSil(int id)
        {
            var result = await _subeServisi.SubeSilAsync(id);
            return result ? NoContent() : NotFound();
        }
    }
}

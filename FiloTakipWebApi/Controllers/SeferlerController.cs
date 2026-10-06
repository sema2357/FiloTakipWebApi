using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SeferlerController : ControllerBase
{
    private readonly ISeferServisi _seferServisi;

    public SeferlerController(ISeferServisi seferServisi)
    {
        _seferServisi = seferServisi;
    }

    /// <summary>Tüm seferleri listele</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SeferDto>>> TumSeferler()
    {
        var sonuc = await _seferServisi.TumSeferleriGetirAsync();
        return Ok(sonuc);
    }

    /// <summary>Sefer getir</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SeferDto>> SeferGetir(int id)
    {
        var sonuc = await _seferServisi.SeferGetirAsync(id);

        return sonuc is null
            ? NotFound(new HataYanitiDto($"ID={id} sefer bulunamadı."))
            : Ok(sonuc);
    }

    /// <summary>Yeni sefer oluştur</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Yonetici")]
    public async Task<ActionResult<SeferDto>> SeferOlustur(
        [FromBody] SeferOlusturDto dto)
    {
        var sonuc = await _seferServisi.SeferOlusturAsync(dto);

        return CreatedAtAction(nameof(SeferGetir), new { id = sonuc.Id }, sonuc);
    }

    /// <summary>Sefer onayla / reddet</summary>
    [HttpPost("onay")]
    [Authorize(Roles = "Admin,Yonetici")]
    public async Task<ActionResult> SeferOnayla(
        [FromBody] SeferOnayDto dto)
    {
        var sonuc = await _seferServisi.SeferOnaylaAsync(dto);

        return sonuc
            ? NoContent()
            : BadRequest(new HataYanitiDto("Sefer onay işlemi başarısız."));
    }

    /// <summary>Sefer sil</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SeferSil(int id)
    {
        var sonuc = await _seferServisi.SeferSilAsync(id);

        return sonuc ? NoContent() : NotFound();
    }
}
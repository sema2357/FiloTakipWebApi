using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SoforlerController : ControllerBase
{
    private readonly ISoforServisi _soforServisi;

    public SoforlerController(ISoforServisi soforServisi)
    {
        _soforServisi = soforServisi;
    }

    /// <summary>Tüm şoförleri listele</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SoforDto>>> TumSoforler()
    {
        var sonuc = await _soforServisi.TumSoforleriGetirAsync();
        return Ok(sonuc);
    }

    /// <summary>Şoför getir</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SoforDto>> SoforGetir(int id)
    {
        var sonuc = await _soforServisi.SoforGetirAsync(id);

        return sonuc is null
            ? NotFound(new HataYanitiDto($"ID={id} şoför bulunamadı."))
            : Ok(sonuc);
    }

    /// <summary>Şoför oluştur</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Yonetici")]
    public async Task<ActionResult<SoforDto>> SoforEkle(
        [FromBody] SoforOlusturDto dto)
    {
        var sonuc = await _soforServisi.SoforOlusturAsync(dto);

        return CreatedAtAction(nameof(SoforGetir), new { id = sonuc.Id }, sonuc);
    }

    /// <summary>Şoför güncelle</summary>
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Admin,Yonetici")]
    public async Task<ActionResult<SoforDto>> SoforGuncelle( int id, [FromBody] SoforGuncelleDto dto)
    {
        var sonuc = await _soforServisi.SoforGuncelleAsync(id, dto);

        return sonuc is null
            ? NotFound()
            : Ok(sonuc);
    }

    /// <summary>Şoför sil</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SoforSil(int id)
    {
        var sonuc = await _soforServisi.SoforSilAsync(id);

        return sonuc ? NoContent() : NotFound();
    }
}
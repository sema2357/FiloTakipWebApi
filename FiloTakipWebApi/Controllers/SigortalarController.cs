using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SigortalarController : ControllerBase
{
    private readonly ISigortaServisi _sigortaServisi;

    public SigortalarController(ISigortaServisi sigortaServisi)
    {
        _sigortaServisi = sigortaServisi;
    }

    /// <summary>Tüm sigorta poliçeleri</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SigortaPolicesiDto>>> TumSigortalar()
    {
        var sonuc = await _sigortaServisi.TumSigortalariGetirAsync();
        return Ok(sonuc);
    }

    /// <summary>Sigorta getir</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<SigortaPolicesiDto>> SigortaGetir(int id)
    {
        var sonuc = await _sigortaServisi.SigortaGetirAsync(id);

        return sonuc is null
            ? NotFound(new HataYanitiDto($"ID={id} poliçe bulunamadı."))
            : Ok(sonuc);
    }

    /// <summary>Yeni sigorta ekle</summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Yonetici")]
    public async Task<ActionResult<SigortaPolicesiDto>> SigortaEkle(
        [FromBody] SigortaPolicesiOlusturDto dto)
    {
        var sonuc = await _sigortaServisi.SigortaOlusturAsync(dto);

        return CreatedAtAction(nameof(SigortaGetir), new { id = sonuc.Id }, sonuc);
    }

    /// <summary>Sigorta sil</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> SigortaSil(int id)
    {
        var sonuc = await _sigortaServisi.SigortaSilAsync(id);

        return sonuc ? NoContent() : NotFound();
    }
}
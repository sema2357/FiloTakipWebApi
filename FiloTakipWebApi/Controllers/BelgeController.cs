using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FiloTakipWebApi.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BelgelerController : ControllerBase
{
    private readonly IBelgeServisi _belgeServisi;

    public BelgelerController(IBelgeServisi belgeServisi)
    {
        _belgeServisi = belgeServisi;
    }

    /// <summary>Tüm belgeleri listele</summary>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<BelgeDto>>> TumBelgeler()
    {
        var sonuc = await _belgeServisi.TumBelgeleriGetirAsync();
        return Ok(sonuc);
    }

    /// <summary>Araca ait belgeleri getir</summary>
    [HttpGet("arac/{aracId:int}")]
    public async Task<ActionResult<IEnumerable<BelgeDto>>> AracaAitBelgeler(int aracId)
    {
        var sonuc = await _belgeServisi.AracaAitBelgeleriGetirAsync(aracId);
        return Ok(sonuc);
    }

    /// <summary>Belge getir</summary>
    [HttpGet("{id:int}")]
    public async Task<ActionResult<BelgeDto>> BelgeGetir(int id)
    {
        var belge = await _belgeServisi.BelgeGetirAsync(id);

        return belge is null
            ? NotFound(new HataYanitiDto($"ID={id} belge bulunamadı."))
            : Ok(belge);
    }

    /// <summary>Yeni belge ekle</summary>
    [HttpPost]
    public async Task<ActionResult<BelgeDto>> BelgeEkle([FromBody] BelgeOlusturDto dto)
    {
        var belge = await _belgeServisi.BelgeEkleAsync(dto);
        return Created("", belge);
    }

    /// <summary>Belge sil</summary>
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Admin,Yonetici")]
    public async Task<ActionResult> BelgeSil(int id)
    {
        var sonuc = await _belgeServisi.BelgeSilAsync(id);

        return sonuc ? NoContent() : NotFound();
    }
}
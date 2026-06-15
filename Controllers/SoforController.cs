using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiloTakipWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SoforController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SoforController(AppDbContext context)
        {
            _context = context;
        }

        // ŞOFÖRLERİ LİSTELE (Görev Geçmişi & Performans Puanı takibi için)
        [HttpGet]
        public async Task<IActionResult> TumSoforleriGetir()
        {
            var soforler = await _context.Soforler.Include(s => s.AktifArac).ToListAsync();
            return Ok(soforler);
        }

        // YENİ ŞOFÖR KAYDI (Şoför Kayıt & Profil için)
        [HttpPost]
        public async Task<IActionResult> SoforEkle([FromBody] SoforEkleDto dto)
        {
            var yeniSofor = new Sofor
            {
                Ad = dto.Ad,
                Soyad = dto.Soyad,
                EhliyetSinifi = dto.EhliyetSinifi.ToUpper(),
                EhliyetGecerlilikTarihi = dto.EhliyetGecerlilikTarihi,
                TelefonNo = dto.TelefonNo,
                PerformansPuani = 100, // Performans Puanı başlangıç değeri
                AktifMi = true
            };

            _context.Soforler.Add(yeniSofor);
            await _context.SaveChangesAsync();

            return Ok(new { Mesaj = "Şoför başarıyla kaydedildi.", SoforId = yeniSofor.Id });
        }

        // ARAÇ - ŞOFÖR EŞLEŞTİRME (Araç-Şoför Eşleştirme için)
        [HttpPut("{id}/arac-ata")]
        public async Task<IActionResult> AracAta(int id, [FromBody] AracEslesmeDto dto)
        {
            var sofor = await _context.Soforler.FindAsync(id);
            if (sofor == null)
            {
                return NotFound("Şoför bulunamadı.");
            }

            if (dto.AracId.HasValue)
            {
                // Araç gerçekten var mı?
                var arac = await _context.Araclar.FindAsync(dto.AracId.Value);
                if (arac == null)
                {
                    return NotFound("Atanmak istenen araç bulunamadı.");
                }

                //Çift atamayı engelleme
                var aracKullanimdaMi = await _context.Soforler
                    .AnyAsync(s => s.AktifAracId == dto.AracId.Value && s.Id != id);

                if (aracKullanimdaMi)
                {
                    return BadRequest("Bu araç şu anda başka bir şoföre atanmış durumda.");
                }
            }

            // Eşleştirmeyi güncelle
            sofor.AktifAracId = dto.AracId;
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Mesaj = dto.AracId.HasValue ? "Araç şoföre başarıyla atandı." : "Şoförün araç ataması kaldırıldı."
            });
        }
    }
}
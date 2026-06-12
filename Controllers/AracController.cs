using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Entities;
using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiloTakipWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AracController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AracController(AppDbContext context)
        {
            _context = context;
        }

        // TÜM ARAÇLARI LİSTELE (Araç Durum Takibi ve Profil Listesi için)
        [HttpGet]
        public async Task<IActionResult> TumAraclariGetir()
        {
            var araclar = await _context.Araclar.ToListAsync();
            return Ok(araclar);
        }

        // YENİ ARAÇ KAYDI (Araç Kayıt & Profil için)
        [HttpPost]
        public async Task<IActionResult> AracEkle([FromBody] AracEkleDto dto)
        {
            // Aynı plakaya sahip başka bir araç var mı kontrolü
            var plakaVarMi = await _context.Araclar.AnyAsync(a => a.Plaka == dto.Plaka);
            if (plakaVarMi)
            {
                return BadRequest("Bu plaka ile kayıtlı bir araç zaten var.");
            }

            var yeniArac = new Arac
            {
                Plaka = dto.Plaka.ToUpper().Replace(" ", ""),
                Marka = dto.Marka,
                Model = dto.Model,
                Yil = dto.Yil,
                SasiNo = dto.SasiNo,
                RuhsatNo = dto.RuhsatNo,
                GuncelKm = dto.GuncelKm,
                AktifMi = true
            };

            _context.Araclar.Add(yeniArac);
            await _context.SaveChangesAsync();

            return Ok(new { Mesaj = "Araç başarıyla kaydedildi.", AracId = yeniArac.Id });
        }

        // KİLOMETRE GÜNCELLEMESİ
        [HttpPut("{id}/kilometre-guncelle")]
        public async Task<IActionResult> KilometreGuncelle(int id, [FromBody] AracKmGuncelleDto dto)
        {
            var arac = await _context.Araclar.FindAsync(id);
            if (arac == null)
            {
                return NotFound("Araç bulunamadı.");
            }

            // Kilometre geriye doğru gidemez kontrolü
            if (dto.YeniKilometre < arac.GuncelKm)
            {
                return BadRequest($"Yeni kilometre, aracın mevcut kilometresinden ({arac.GuncelKm}) düşük olamaz.");
            }

            arac.GuncelKm = dto.YeniKilometre;
            await _context.SaveChangesAsync();

            return Ok(new { Mesaj = "Aracın kilometresi güncellendi.", GuncelKm = arac.GuncelKm });
        }
    }
}
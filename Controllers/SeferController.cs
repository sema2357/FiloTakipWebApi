using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using FiloTakipWebApi.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiloTakipWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SeferController : ControllerBase
    {
        private readonly AppDbContext _context;

        public SeferController(AppDbContext context)
        {
            _context = context;
        }
        //Seferleri listele
        [HttpGet("aktif-seferler")]
        public async Task<IActionResult> AktifSeferleriGetir()
        {
            var seferler = await _context.Seferler.Include(s => s.Arac)
                .Include(s => s.Sofor)
                .Where(s => s.Durum == "Aktif")
                .ToListAsync();
            return Ok(seferler);
        }

        //Yeni Sefer Başlat
        [HttpPost("sefer-baslat")]
        public async Task<IActionResult> SeferBaslat([FromBody] SeferBaslatDto dto)
        {
            //araç var mı?, boşta mı?
            var arac = await _context.Araclar.FindAsync(dto.AracId);

            if (arac == null)
            {
                return BadRequest("Sefer başlatılamadı: Belirtilen araç bulunamadı.");
            }

            if (!arac.AktifMi)
            {
                return BadRequest("Sefer başlatılamadı: Bu araç şu anda aktif değil.");
            }

            var sofor = await _context.Soforler.FindAsync(dto.SoforId);
            if (sofor == null)
            {
                return BadRequest("Sefer başlatılamadı: Belirtilen şoför bulunamadı.");
            }

            if (!sofor.AktifMi)
            {
                return BadRequest("Sefer başlatılamadı: Bu şoför şu anda aktif değil.");
            }

            var aracMeşgulMu = await _context.Seferler.AnyAsync(s => s.AracId == dto.AracId && s.Durum == "Aktif");
            if (aracMeşgulMu)
            {
                return BadRequest("Bu araç şu anda zaten aktif bir seferde.");
            }

            var yeniSefer = new Sefer
            {
                AracId = dto.AracId,
                SoforId = dto.SoforId,
                CikisNoktasi = dto.CikisNoktasi,
                VarisNoktasi = dto.VarisNoktasi,
                BaslangicKm = arac.GuncelKm,
                Durum = "Aktif"
            };

            _context.Seferler.Add(yeniSefer);
            await _context.SaveChangesAsync();

            return Ok(new { Mesaj = "Sefer başarıyla başlatıldı ve araç yola çıktı.", SeferId = yeniSefer.Id });
        }

        [HttpPut("{id}/sefer-bitir")]
        public async Task<IActionResult> SeferBitir(int id, [FromBody] SeferBitirDto dto)
        {
            var sefer = await _context.Seferler.Include(s => s.Arac).FirstOrDefaultAsync(s =>s.Id == id);
            if (sefer == null || sefer.Durum == "Aktif") return NotFound("Aktif sefer bulunamadı"); 
            
            //Bitiş km başlangıç km den az olamaz.
            if (dto.BitisKm<sefer.BaslangicKm)
            {
                return BadRequest($"Bitiş kilometresi, başlangıç kilometresinden ({sefer.BaslangicKm}) az olamaz.");
            }

            // Seferi güncelle
            sefer.BitisKm = dto.BitisKm;
            sefer.BitisTarihi = DateTime.Now;
            sefer.Durum = "Tamamlandı";

            //Aracın güncel kilometresini de sefer bittiği için yeniliyoruz.
            sefer.Arac.GuncelKm = dto.BitisKm;

            await _context.SaveChangesAsync();

            int katEdilenKm = dto.BitisKm - sefer.BaslangicKm;
            return Ok(new { Mesaj = "Sefer başarıyla tamamlandı.", KatEdilenMesafe = $"{katEdilenKm} KM" });
        
    }


    }
}

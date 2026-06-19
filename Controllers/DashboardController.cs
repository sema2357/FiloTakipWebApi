using FiloTakipWebApi.Data;
using FiloTakipWebApi.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiloTakipWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController: ControllerBase
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet("ozet")]
        public async Task<IActionResult> GetOzetIstatikler()
        {
            var toplamArac = await _context.Araclar.CountAsync();
            var seferdekiArac = await _context.Seferler.Where(s => s.Durum == "Aktif").Select(s => s.AracId).Distinct().CountAsync();
            var musaitArac = toplamArac - seferdekiArac;

            var toplamSofor = await _context.Soforler.CountAsync();
            var soforVarMi = await _context.Soforler.AnyAsync();
            var performansOrtalamasi = soforVarMi
                ? await _context.Soforler.AverageAsync(s => s.PerformansPuani)
                :0;

            var tamamlananSeferler = await _context.Seferler.Where(s => s.Durum == "Tamamlandı").ToListAsync();
            var toplamSeferSayisi = tamamlananSeferler.Count;

            int toplamKm = tamamlananSeferler.Sum(s => (s.BitisKm ?? 0) - (s.BaslangicKm));

            var dashboardData = new DashboardOzetDto
            {
                ToplamAracSayisi = toplamArac,
                SeferdekiAracSayisi = seferdekiArac,
                MusaitAracSayisi = musaitArac,
                ToplamSoforSayisi = toplamSofor,
                SoforPerformansOrtalamasi = performansOrtalamasi,
                ToplamTamamlananSeferSayisi = toplamSeferSayisi,
                ToplamKatedilenMesafeKm = toplamKm,
            };
            return Ok(dashboardData);
        }

    }
}
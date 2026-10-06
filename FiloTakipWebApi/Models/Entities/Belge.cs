using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.Models.Entities
{
    public class Belge
    {
        public int Id { get; set; }
        public int AracId { get; set; }
        public BelgeTipi BelgeTipi { get; set; }
        public string BelgeAdi { get; set; } = null!;
        public string DosyaUrl { get; set; } = null!;
        public DateTime? GecerlilikTarihi { get; set; }
        public bool UyariBildirimMi { get; set; } = true;

        public bool AktifMi { get; set; }
        public int? UyariGunSayisi { get; set; } = 30;
        public DateTime YuklenmeTarihi { get; set; } = DateTime.Now;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public Arac Arac { get; set; } = null!;
    }
}

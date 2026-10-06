using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.Models.Entities
{
    public class YakitGirisi
    {
        public int Id { get; set; }
        public int AracId { get; set; }
        public int? SoforId { get; set; }
        public DateTime GirisTarihi { get; set; }
        public decimal Litre { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal ToplamTutar { get; set; }
        public int KmOkumasi { get; set; }
        public string? PompaAdi { get; set; }
        public string? IstasyonAdi { get; set; }
        public string? YakitKartiNo { get; set; }
        public YakitTipi YakitTipi { get; set; }
        public decimal? Litreper100Km { get; set; }   // hesaplanmış
        public bool AnormalTuketimMi { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Arac Arac { get; set; } = null!;
        public Sofor? Sofor { get; set; }
    }
}

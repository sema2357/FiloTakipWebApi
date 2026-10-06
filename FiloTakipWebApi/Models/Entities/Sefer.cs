using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.Models.Entities
{
    public class Sefer
    {
        public int Id { get; set; }
        public int AracId { get; set; }
        public int SoforId { get; set; }
        public string BaslangicNoktasi { get; set; } = null!;
        public string VarisNoktasi { get; set; } = null!;
        public DateTime PlanlananBaslangic { get; set; }
        public DateTime PlanlananBitis { get; set; }
        public DateTime? GercekBaslangic { get; set; }
        public DateTime? GercekBitis { get; set; }
        public int PlanlananKm { get; set; }
        public int? GercekKm { get; set; }
        public string? YukBilgisi { get; set; }
        public string? YolcuBilgisi { get; set; }
        public SeferDurumu Durumu { get; set; } = SeferDurumu.Planlandi;

        public bool OnaylandiMi { get; set; } = false;

        public bool AktifMi { get; set; }
        public int? OnaylayanKullaniciId { get; set; }
        public string? SeferBelgesiUrl { get; set; }
        public string? IrsaliyeNo { get; set; }
        public string? Notlar { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;

        public string? RedGerekce {  get; set; }

        public Arac Arac { get; set; } = null!;
        public Sofor Sofor { get; set; } = null!;

    }
}

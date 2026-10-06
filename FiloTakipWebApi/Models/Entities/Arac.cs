using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.Models.Entities
{
    public class Arac
    {
        public int Id { get; set; }

        public required string Plaka { get; set; } 

        public required string Marka { get; set; }

        public  required string Model { get; set; }

        public int Yil {  get; set; }

        public string? SasiNo { get; set; }

        public YakitTipi YakitTipi { get; set; }

        public AracDurumu AracDurumu { get; set; }

        public int GuncelKm { get; set; }

        public string? GorselUrl { get; set; }

        public int? SubeId { get; set; }

        public bool AktifMi { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public DateTime UpdatedAt { get; set; }

        public Sube? Sube { get; set; }

        public ICollection<AracSoforAtama> AracSoforAtamalari { get; set; } = new List<AracSoforAtama>();

        public ICollection<Sefer> Seferler { get; set; } = new List<Sefer>();

        public ICollection<YakitGirisi> YakitGirisleri { get; set; } = [];

        public ICollection<BakimKaydi> BakimKayitlari { get; set; } = [];

        public ICollection<Belge> Belgeler { get; set; } = [];

        public ICollection<SigortaPolicesi> SigortaPoliceleri { get; set; } = [];
}
}

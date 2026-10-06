using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.Models.Entities
{
    public class BakimKaydi
    {
        public int Id { get; set; }
        public int AracId { get; set; }
        public BakimTipi BakimTipi { get; set; }
        public string Baslik { get; set; } = null!;
        public string? Aciklama { get; set; }
        public DateTime BakimTarihi { get; set; }
        public DateTime? TamamlanmaTarihi { get; set; }
        public int KmOkumasi { get; set; }
        public int? SonrakiBakimKm { get; set; }
        public DateTime? SonrakiBakimTarihi { get; set; }
        public string? ServisAdi { get; set; }
        public decimal? MaliyetToplam { get; set; }
        public bool ArizaKaydiMi { get; set; } = false;
        public bool TamamlandiMi { get; set; } = false;
        public string? Notlar { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Arac Arac { get; set; } = null!;
        public ICollection<BakimParcaDegisimi> ParcaDegisimleri { get; set; } = new List<BakimParcaDegisimi>();
        public ICollection<BakimMasrafi> Masraflar { get; set; } = new List<BakimMasrafi>();

    }
}

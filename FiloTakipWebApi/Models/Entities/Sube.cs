namespace FiloTakipWebApi.Models.Entities
{
    public class Sube
    {
        public int Id { get; set; }
        public string Ad { get; set; } = null!;
        public string? Adres { get; set; }
        public string? Sehir { get; set; }
        public string? Telefon { get; set; }
        public bool AktifMi { get; set; } = true;

        //public ICollection<Arac> Araclar { get; set; } = new List<Arac>();
        public ICollection<Arac> Araclar { get; set; } = [];
        public ICollection<Kullanici> Kullanicilar { get; set; } = [];

    }
}

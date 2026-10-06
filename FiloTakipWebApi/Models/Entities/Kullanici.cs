using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.Models.Entities
{
    public class Kullanici
    {
        public int Id { get; set; }
        public string AdSoyad { get; set; } = null!;
        public string Eposta { get; set; } = null!;
        public string SifreHash { get; set; } = null!;
        public KullaniciRolu Rol { get; set; } = KullaniciRolu.Yonetici;
        public int? SubeId { get; set; }
        public bool AktifMi { get; set; } = true;
        public DateTime? CreatedAt { get; set; } = DateTime.Now;

        public DateTime? SonGirisTarihi { get; set; }

        public Sube? Sube { get; set; }
    }
}

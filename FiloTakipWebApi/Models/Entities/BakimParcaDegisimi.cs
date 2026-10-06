namespace FiloTakipWebApi.Models.Entities
{
    public class BakimParcaDegisimi
    {
        public int Id { get; set; }
        public int BakimKaydiId { get; set; }
        public string ParcaAdi { get; set; } = null!;
        public int Adet { get; set; } = 1;
        public decimal BirimFiyat { get; set; }
        public decimal ToplamFiyat { get; set; }
        public string? ParcaKodu { get; set; }

        public BakimKaydi BakimKaydi { get; set; } = null!;
    }
}

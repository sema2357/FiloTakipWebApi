namespace FiloTakipWebApi.Models.Entities
{
    public class BakimMasrafi
    {
        public int Id { get; set; }
        public int BakimKaydiId { get; set; }
        public string MasrafKategorisi { get; set; } = null!;
        public string Aciklama { get; set; } = null!;
        public decimal Tutar { get; set; }

        public BakimKaydi BakimKaydi { get; set; } = null!;
    }
}

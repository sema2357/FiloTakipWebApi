namespace FiloTakipWebApi.Models.Entities
{
    public class CezaIhlalKaydi
    {
        public int Id { get; set; }
        public int SoforId { get; set; }
        public int? AracId { get; set; }
        public string CezaTuru { get; set; } = null!;
        public string? Aciklama { get; set; }
        public decimal? Tutar { get; set; }
        public DateTime OlayTarihi { get; set; }
        public bool OdenmisMi { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Sofor Sofor { get; set; } = null!;
        public Arac? Arac { get; set; }
    }
}


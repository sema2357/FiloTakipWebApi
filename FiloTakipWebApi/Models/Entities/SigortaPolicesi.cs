namespace FiloTakipWebApi.Models.Entities
{
    public class SigortaPolicesi
    {
        public int Id { get; set; }
        public int AracId { get; set; }
        public string SigortaSirketi { get; set; } = null!;
        public string PoliceNo { get; set; } = null!;
        public DateTime BaslangicTarihi { get; set; }
        public DateTime BitisTarihi { get; set; }
        public decimal Prim { get; set; }
        public string? SigortaTuru { get; set; }
        public string? DosyaUrl { get; set; }
        public bool AktifMi { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public Arac Arac { get; set; } = null!;

    }
}

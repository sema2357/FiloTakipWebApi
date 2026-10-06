namespace FiloTakipWebApi.Models.Entities
{
    public class BildirimSablonu
    {
        public int Id { get; set; }
        public string Ad { get; set; } = null!;
        public string Konu { get; set; } = null!;
        public string IcerikSablonu { get; set; } = null!;
        public string OlayTipi { get; set; } = null!;   // BakimHatirlatma, BelgeGecerlilik vs.
        public bool AktifMi { get; set; } = true;
    }
}

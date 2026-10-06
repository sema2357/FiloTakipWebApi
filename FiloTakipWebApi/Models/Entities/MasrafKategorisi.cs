namespace FiloTakipWebApi.Models.Entities
{
    public class MasrafKategorisi
    {
        public int Id { get; set; }
        public string Ad { get; set; } = null!;
        public string? Aciklama { get; set; }

        public decimal? Tutar {  get; set; }
        public bool AktifMi { get; set; } = true;
    }
}

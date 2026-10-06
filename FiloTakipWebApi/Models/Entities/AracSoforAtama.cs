
namespace FiloTakipWebApi.Models.Entities
{
    public class AracSoforAtama
    {
        public int Id { get; set; }
        public required int AracId { get; set; }    
        public Arac Arac { get; set; } = null!;

        public int SoforId { get; set; }

        public Sofor Sofor { get; set; } = null!;
        public DateTime BaslangicTarihi { get; set; }
        public DateTime? BitisTarihi { get; set; }
        public bool AktifMi { get; set; } = true;
        
      
    }
}

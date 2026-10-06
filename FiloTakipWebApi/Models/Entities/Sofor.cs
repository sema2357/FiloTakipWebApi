using FiloTakipWebApi.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace FiloTakipWebApi.Models.Entities
{
    public class Sofor
    {
        [Key]
        public int Id { get; set; }

        public required string Ad {  get; set; }

        public required string Soyad { get; set; }

        public string? TcNo { get; set; }

        public string? Tel {  get; set; }

        public string? Eposta { get; set; }

        public DateTime DogumTarihi { get; set; }

        public string? EhliyetNo { get; set; }

        public required string EhliyetSinifi {  get; set; }

        public DateTime EhliyetGecerlilikTarihi { get; set; }
        public decimal PerformansPuani { get; set; } = 100;
        public SoforDurumu SoforDurumu { get; set; } 

        public bool AktifMi {  get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        public ICollection<Sefer> Seferler { get; set; } = new List<Sefer>();
        public ICollection<AracSoforAtama> AracSoforAtamalari { get; set; } = new List<AracSoforAtama>();
        public ICollection<CezaIhlalKaydi> CezaIhlalKayitlari { get; set; } = new List<CezaIhlalKaydi>();


    }
}

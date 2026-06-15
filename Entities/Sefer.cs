using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiloTakipWebApi.Entities
{
    public class Sefer
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int AracId { get; set; }

        [ForeignKey("AracId")]
        public Arac Arac { get; set; }

        [Required]
        public int SoforId { get; set; }

        [ForeignKey("SoforId")]
        public Sofor Sofor { get; set; }

        [Required]
        [StringLength(100)]
        public string CikisNoktasi { get; set; }

        [Required]
        [StringLength(100)]
        public string VarisNoktasi { get; set; }

        public DateTime BaslangicTarihi { get; set; } = DateTime.Now;

        public DateTime? BitisTarihi { get; set; }
        
        public int BaslangicKm { get; set; }
        
        public int? BitisKm { get; set; }

        //Durumu; Aktif, Tamamlandi, İptal
        [StringLength(20)]
        public string Durum { get; set; } = "Aktif";

        public DateTime CreatedAd {  get; set; } = DateTime.Now;



    }
}

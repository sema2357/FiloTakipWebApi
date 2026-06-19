using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FiloTakipWebApi.Entities
{
    public class Sofor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Ad { get; set; }

        [Required]
        [StringLength(50)]
        public string Soyad { get; set; }

        [StringLength(20)]
        public string EhliyetSinifi { get; set; } 

        public DateTime EhliyetGecerlilikTarihi { get; set; } 

        [StringLength(20)]
        public string TelefonNo { get; set; }

        public int PerformansPuani { get; set; } = 100; // Başlangıç 100

        public bool AktifMi { get; set; } = true;

        //  Araç - Şoför Eşleştirme
        public int? AktifAracId { get; set; }

        [ForeignKey("AktifAracId")]
        public Arac AktifArac { get; set; }
    }
}

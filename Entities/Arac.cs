using System;
using System.ComponentModel.DataAnnotations;


namespace FiloTakipWebApi.Entities
{
    public class Arac
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Plaka { get; set; }

        [Required]
        [StringLength(50)]
        public string Marka { get; set; }

        [Required]
        [StringLength (50)]
        public string Model { get; set; }

        public int Yil { get; set; }

        [StringLength(50)]
        public string SasiNo { get; set; }

        [StringLength (50)]
        public string RuhsatNo { get; set; }

        public int GuncelKm { get; set; } 

        public bool AktifMi { get; set; } = true; // Araç durum takibi için aktif/pasif durumu

        // Görseldeki "Fotoğraf galerisi" için şimdilik bir klasör yolu veya URL tutacağız
        public string? GorselUrl { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;









    }
}

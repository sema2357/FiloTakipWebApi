//Araç kaydederken kullanıcıdan istenecek alanların DTO su.

using System.ComponentModel.DataAnnotations;

namespace FiloTakipWebApi.DTOs
{
    public class AracEkleDto
    {
        [Required(ErrorMessage = "Plaka alanı zorunludur.")]
        [StringLength(20)]
        public string Plaka { get; set; }

        [Required(ErrorMessage = "Marka alanı zorunludur.")]
        public string Marka { get; set; }

        [Required(ErrorMessage = "Model alanı zorunludur.")]
        public string Model { get; set; }

        public int Yil { get; set; }
        public string SasiNo { get; set; }
        public string RuhsatNo { get; set; }
        public int GuncelKm { get; set; }
    }
}
using System;
using System.ComponentModel.DataAnnotations;

namespace FiloTakipWebApi.DTOs
{
    public class SoforEkleDto
    {
        [Required(ErrorMessage = "Şoför adı zorunludur.")]
        public string Ad { get; set; }

        [Required(ErrorMessage = "Şoför soyadı zorunludur.")]
        public string Soyad { get; set; }

        [Required(ErrorMessage = "Ehliyet sınıfı zorunludur.")]
        public string EhliyetSinifi { get; set; }

        public DateTime EhliyetGecerlilikTarihi { get; set; }

        public string TelefonNo { get; set; }
    }
}
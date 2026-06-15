using System.ComponentModel.DataAnnotations;

namespace FiloTakipWebApi.DTOs
{
    public class SeferBaslatDto
    {
        [Required]
        public int AracId { get; set; }

        [Required]
        public int SoforId { get; set; }

        [Required]
        public string CikisNoktasi { get; set; }

        [Required]
        public string VarisNoktasi { get; set; }
    }
}

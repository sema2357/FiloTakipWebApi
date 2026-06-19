using System.ComponentModel.DataAnnotations;

namespace FiloTakipWebApi.Entities
{
    public class Kullanici
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string KullaniciAdi { get; set; }

        [Required]
        public string SifreHash { get; set; }

        [Required]
        public string Rol { get; set; }

    }
}

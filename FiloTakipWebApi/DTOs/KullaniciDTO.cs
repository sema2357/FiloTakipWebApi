using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.DTOs
{
    public record GirisIstegiDto(string Eposta, string Sifre);

    public record GirisYanitiDto(string Token, string AdSoyad, KullaniciRolu Rol, int? SubeId);

    public record KullaniciDto(int Id, string AdSoyad, string Eposta, KullaniciRolu Rol, int? SubeId);

    public record KullaniciOlusturDto(
        string AdSoyad,
        string Eposta,
        string Sifre,
        KullaniciRolu Rol,
        int? SubeId
    );
}

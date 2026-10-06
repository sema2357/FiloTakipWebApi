namespace FiloTakipWebApi.DTOs
{
    public record SubeDto(int Id, string Ad, string? Adres, string? Sehir, string? Telefon, bool AktifMi);
    public record SubeOlusturDto(string Ad, string? Adres, string? Sehir, string? Telefon);
}

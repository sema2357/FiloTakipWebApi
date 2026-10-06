using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.DTOs
{
    public record BakimKaydiOlusturDto(
     int AracId,
     BakimTipi BakimTipi,
     string Baslik,
     string? Aciklama,
     DateTime BakimTarihi,
     int KmOkumasi,
     int? SonrakiBakimKm,
     DateTime? SonrakiBakimTarihi,
     string? ServisAdi,
     bool ArizaKaydiMi,
     string? Notlar
    );

    public record BakimKaydiDto(
        int Id,
        string AracPlaka,
        BakimTipi BakimTipi,
        string Baslik,
        DateTime BakimTarihi,
        DateTime? TamamlanmaTarihi,
        int KmOkumasi,
        int? SonrakiBakimKm,
        string? ServisAdi,
        decimal? MaliyetToplam,
        bool TamamlandiMi
    );
}

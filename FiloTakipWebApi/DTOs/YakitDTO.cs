using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.DTOs
{
    public record YakitGirisiOlusturDto(
    int AracId,
    int? SoforId,
    DateTime GirisTarihi,
    decimal Litre,
    decimal BirimFiyat,
    int KmOkumasi,
    string? PompaAdi,
    string? IstasyonAdi,
    string? YakitKartiNo,
    YakitTipi YakitTipi
);

    public record YakitGirisiDto(
        int Id,
        string AracPlaka,
        string? SoforAdSoyad,
        DateTime GirisTarihi,
        decimal Litre,
        decimal BirimFiyat,
        decimal ToplamTutar,
        int KmOkumasi,
        string? IstasyonAdi,
        decimal? Litreper100Km,
        bool AnormalTuketimMi
    );

}

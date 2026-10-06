using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.DTOs
{
    public record AracOlusturDto(
      string Plaka,
      string Marka,
      string Model,
      int Yil,
      string? SasiNo,
      YakitTipi YakitTipi,
      int? SubeId
    );

    public record AracGuncelleDto(
        string Marka,
        string Model,
        int Yil,
        string? SasiNo,
        YakitTipi YakitTipi,
        AracDurumu AracDurumu,
        int GuncelKm,
        int? SubeId
    );

    public record KmGuncelleDto(
        int YeniKm
        );

    public record AracDto(
        int Id,
        string Plaka,
        string Marka,
        string Model,
        int Yil,
        string? SasiNo,
        YakitTipi YakitTipi,
        AracDurumu Durumu,
        int GuncelKilometre,
        string? FotografUrl,
        string? SubeAdi,
        DateTime CreatedAt
    );

}

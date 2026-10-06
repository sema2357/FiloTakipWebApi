using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.DTOs
{
    public record BelgeDto(
    int Id,
    string AracPlaka,
    BelgeTipi BelgeTipi,
    string BelgeAdi,
    string DosyaYolu,
    DateTime? GecerlilikTarihi,
    bool UyariBildirimMi,
    int? UyariGunSayisi,
    DateTime YuklenmeTarihi
    );

    public record BelgeOlusturDto(
        int AracId,
        BelgeTipi BelgeTipi,
        string BelgeAdi,
        string DosyaYolu,
        DateTime? GecerlilikTarihi,
        bool UyariBildirimMi = true,
        int? UyariGunSayisi = 30
    );

    public record SigortaPolicesiOlusturDto(
        int AracId,
        string SigortaSirketi,
        string PoliceNo,
        DateTime BaslangicTarihi,
        DateTime BitisTarihi,
        decimal Prim,
        string? SigortaTuru
    );

    public record SigortaPolicesiDto(
        int Id,
        string AracPlaka,
        string SigortaSirketi,
        string PoliceNo,
        DateTime BaslangicTarihi,
        DateTime BitisTarihi,
        decimal Prim,
        string? SigortaTuru,
        bool AktifMi
    );

}

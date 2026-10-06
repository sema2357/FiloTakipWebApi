using FiloTakipWebApi.Models.Enums;

namespace FiloTakipWebApi.DTOs
{
    public record SeferOlusturDto(
    int AracId,
    int SoforId,
    string BaslangicNoktasi,
    string VarisNoktasi,
    DateTime PlanlananBaslangic,
    DateTime PlanlananBitis,
    int PlanlananKm,
    string? YukBilgisi,
    string? YolcuBilgisi,
    string? IrsaliyeNo,
    string? Notlar
);

    public record SeferDto(
        int Id,
        string AracPlaka,
        string SoforAdSoyad,
        string BaslangicNoktasi,
        string VarisNoktasi,
        DateTime PlanlananBaslangic,
        DateTime PlanlananBitis,
        DateTime? GercekBaslangic,
        DateTime? GercekBitis,
        int PlanlananKm,
        int? GercekKm,
        SeferDurumu Durumu,
        bool OnaylandiMi,
        string? IrsaliyeNo
    );

    public record SeferOnayDto(
        int SeferId, 
        bool Onayla, 
        string? RedGerekce);

}

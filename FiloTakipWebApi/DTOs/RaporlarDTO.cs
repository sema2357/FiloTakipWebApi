namespace FiloTakipWebApi.DTOs
{
    public record AracMaliyetRaporDto(
    string AracPlaka,
    string AracMarka,
    decimal ToplamYakitMaliyeti,
    decimal ToplamBakimMaliyeti,
    decimal GenelToplam,
    int ToplamKm
);

    public record SoforPerformansRaporDto(
        string SoforAdSoyad,
        int TamamlananSefer,
        decimal OrtalamaPerformans,
        int ToplamCeza,
        decimal ToplamCezaTutari
    );

    public record YakitTuketimAnalizDto(
        string AracPlaka,
        decimal OrtalamaLitreper100Km,
        decimal ToplamLitre,
        decimal ToplamMaliyet,
        int AnormalKayitSayisi
    );

}

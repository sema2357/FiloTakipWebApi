namespace FiloTakipWebApi.DTOs
{
    public record SayfalamaliSonucDto<T>(
    IEnumerable<T> Veriler,
    int ToplamKayit,
    int SayfaNo,
    int SayfaBoyutu
);

    public record HataYanitiDto(string Mesaj, string? Detay = null);

    public record FiloGenelOzetDto(
        int ToplamArac,
        int AktifArac,
        int BakimdakiArac,
        int ToplamSofor,
        int BugunkuSefer,
        int DevamEdenSefer,
        decimal AylikYakitMaliyeti,
        decimal AylikBakimMaliyeti
    );
}

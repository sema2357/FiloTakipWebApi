namespace FiloTakipWebApi.DTOs
{
    public record SoforOlusturDto(
    string Ad,
    string Soyad,
    string TcNo,
    string? Tel,
    string? Eposta,
    DateTime DogumTarihi,
    string EhliyetNo,
    string EhliyetSinifi,
    DateTime EhliyetGecerlilikTarihi
);
    public record SoforGuncelleDto(
        string Ad,
        string Soyad,
        string TcNo,
        string? Tel,
        string? Eposta,
        DateTime DogumTarihi,
        string EhliyetNo,
        string EhliyetSinifi,
        DateTime EhliyetGecerlilikTarihi,
        bool AktifMi
    );
    public record SoforDto(
        int Id,
        string Ad,
        string Soyad,
        string TcNo,
        string? Tel,
        string? Eposta,
        string EhliyetNo,
        string EhliyetSinifi,
        DateTime EhliyetGecerlilikTarihi,
        decimal PerformansPuani,
        bool AktifMi
    );

}

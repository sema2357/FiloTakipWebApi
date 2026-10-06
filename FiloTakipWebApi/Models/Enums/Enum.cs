namespace FiloTakipWebApi.Models.Enums
{
    public enum AracDurumu
    {
        Aktif = 1,
        Bakimda = 2,
        Arizali = 3,
        Pasif = 4
    }

    public enum SoforDurumu
    {
        Aktif = 1,
        Pasif = 2,
        İzinde = 3
    }

    public enum SeferDurumu

    {
        Planlandi = 1,
        DevamEdiyor = 2,
        Tamamlandi = 3,
        İptal = 4
    }

    public enum BakimTipi
    {
        Periyodik = 1,
        Ariza = 2,
        LastikDegisimi = 3,
        YagDegisimi = 4,
        Diger = 5
    }

    public enum BelgeTipi
    {
        SigortaPolicesi = 1,
        Ruhsat = 2,
        MuayeneBelgesi = 3,
        EgzozMuayenesi = 4,
        Diger = 5
    }

    public enum YakitTipi
    {
        Benzin = 1,
        Dizel = 2,
        LPG = 3,
        Elektrik = 4,
        Hibrit = 5
    }

    public enum KullaniciRolu
    {
        Admin = 1,
        Yonetici = 2,
        Sofor = 3,
        Muhasebe = 4,
        Teknisyen = 5
    }










}

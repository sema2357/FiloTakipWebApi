namespace FiloTakipWebApi.DTOs
{
    public class DashboardOzetDto
    {
        public int ToplamAracSayisi {  get; set; }
        public int SeferdekiAracSayisi { get; set;}
        public int MusaitAracSayisi { get; set;}

        public int ToplamSoforSayisi { get; set;}
        public double SoforPerformansOrtalamasi { get; set;}

        public int ToplamTamamlananSeferSayisi { get; set;}
        public int ToplamKatedilenMesafeKm {  get; set;}
    }
}

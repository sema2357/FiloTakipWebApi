using FiloTakipWebApi.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace FiloTakipWebApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        // Araç
        public DbSet<Arac> Araclar { get; set; }
        public DbSet<AracSoforAtama> AracSoforAtamalari { get; set; }

        // Şoför
        public DbSet<Sofor> Soforler { get; set; }
        public DbSet<CezaIhlalKaydi> CezaIhlalKayitlari { get; set; }

        // Sefer 
        public DbSet<Sefer> Seferler { get; set; }

        // Yakıt
        public DbSet<YakitGirisi> YakitGirisleri { get; set; }

        // Bakım & Arıza
        public DbSet<BakimKaydi> BakimKayitlari { get; set; }
        public DbSet<BakimParcaDegisimi> BakimParcaDegisimleri { get; set; }
        public DbSet<BakimMasrafi> BakimMasraflari { get; set; }

        // Belge & Sigorta
        public DbSet<Belge> Belgeler { get; set; }
        public DbSet<SigortaPolicesi> SigortaPoliceleri { get; set; }

        // Sistem
        public DbSet<Kullanici> Kullanicilar { get; set; }
        public DbSet<Sube> Subeler { get; set; }
        public DbSet<MasrafKategorisi> MasrafKategorileri { get; set; }
        public DbSet<BildirimSablonu> BildirimSablonlari { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Araç 
            modelBuilder.Entity<Arac>(entity =>
            {
                entity.ToTable("Araclar");
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Plaka).HasMaxLength(20).IsRequired();
                entity.HasIndex(a => a.Plaka).IsUnique();
                entity.Property(a => a.Marka).HasMaxLength(50).IsRequired();
                entity.Property(a => a.Model).HasMaxLength(50).IsRequired();
                entity.Property(a => a.SasiNo).HasMaxLength(50);
                entity.HasOne(a => a.Sube)
                      .WithMany(s => s.Araclar)
                      .HasForeignKey(a => a.SubeId)
                      .OnDelete(DeleteBehavior.SetNull);
            });

            // Şoför
            modelBuilder.Entity<Sofor>(entity =>
            {
                entity.ToTable("Soforler");
                entity.HasKey(s => s.Id);
                entity.Property(s => s.Ad).HasMaxLength(100).IsRequired();
                entity.Property(s => s.Soyad).HasMaxLength(100).IsRequired();
                entity.Property(s => s.TcNo).HasMaxLength(11).IsRequired();
                entity.HasIndex(s => s.TcNo).IsUnique();
                entity.Property(s => s.EhliyetNo).HasMaxLength(20).IsRequired();
                entity.Property(s => s.EhliyetSinifi).HasMaxLength(10).IsRequired();
                entity.Property(s => s.PerformansPuani).HasPrecision(5, 2);
            });

            // Araç-Şoför Atama
            modelBuilder.Entity<AracSoforAtama>(entity =>
            {
                entity.ToTable("AracSoforAtamalari");
                entity.HasOne(a => a.Arac).WithMany(a => a.AracSoforAtamalari)
                      .HasForeignKey(a => a.AracId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(a => a.Sofor).WithMany(s => s.AracSoforAtamalari)
                      .HasForeignKey(a => a.SoforId).OnDelete(DeleteBehavior.Cascade);
            });

            // Sefer
            modelBuilder.Entity<Sefer>(entity =>
            {
                entity.ToTable("Seferler");
                entity.Property(s => s.BaslangicNoktasi).HasMaxLength(200).IsRequired();
                entity.Property(s => s.VarisNoktasi).HasMaxLength(200).IsRequired();
                entity.Property(s => s.IrsaliyeNo).HasMaxLength(50);
                entity.HasOne(s => s.Arac).WithMany(a => a.Seferler)
                      .HasForeignKey(s => s.AracId).OnDelete(DeleteBehavior.Restrict);
                entity.HasOne(s => s.Sofor).WithMany(so => so.Seferler)
                      .HasForeignKey(s => s.SoforId).OnDelete(DeleteBehavior.Restrict);
            });

            // Yakıt Girişi 
            modelBuilder.Entity<YakitGirisi>(entity =>
            {
                entity.ToTable("YakitGirisleri");
                entity.Property(y => y.Litre).HasPrecision(8, 2);
                entity.Property(y => y.BirimFiyat).HasPrecision(8, 2);
                entity.Property(y => y.ToplamTutar).HasPrecision(10, 2);
                entity.Property(y => y.Litreper100Km).HasPrecision(6, 2);
                entity.HasOne(y => y.Arac).WithMany(a => a.YakitGirisleri)
                      .HasForeignKey(y => y.AracId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(y => y.Sofor).WithMany()
                      .HasForeignKey(y => y.SoforId).OnDelete(DeleteBehavior.SetNull);
            });

            // Bakım Kaydı
            modelBuilder.Entity<BakimKaydi>(entity =>
            {
                entity.ToTable("BakimKayitlari");
                entity.Property(b => b.Baslik).HasMaxLength(200).IsRequired();
                entity.Property(b => b.ServisAdi).HasMaxLength(200);
                entity.Property(b => b.MaliyetToplam).HasPrecision(12, 2);
                entity.HasOne(b => b.Arac).WithMany(a => a.BakimKayitlari)
                      .HasForeignKey(b => b.AracId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BakimParcaDegisimi>(entity =>
            {
                entity.ToTable("BakimParcaDegisimleri");
                entity.Property(b => b.ParcaAdi).HasMaxLength(200).IsRequired();
                entity.Property(b => b.BirimFiyat).HasPrecision(10, 2);
                entity.Property(b => b.ToplamFiyat).HasPrecision(12, 2);
                entity.HasOne(b => b.BakimKaydi).WithMany(bk => bk.ParcaDegisimleri)
                      .HasForeignKey(b => b.BakimKaydiId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<BakimMasrafi>(entity =>
            {
                entity.ToTable("BakimMasraflari");
                entity.Property(b => b.Tutar).HasPrecision(10, 2);
                entity.HasOne(b => b.BakimKaydi).WithMany(bk => bk.Masraflar)
                      .HasForeignKey(b => b.BakimKaydiId).OnDelete(DeleteBehavior.Cascade);
            });

            // Belge
            modelBuilder.Entity<Belge>(entity =>
            {
                entity.ToTable("Belgeler");
                entity.Property(b => b.BelgeAdi).HasMaxLength(200).IsRequired();
                entity.Property(b => b.DosyaUrl).HasMaxLength(500).IsRequired();
                entity.HasOne(b => b.Arac).WithMany(a => a.Belgeler)
                      .HasForeignKey(b => b.AracId).OnDelete(DeleteBehavior.Cascade);
            });

            // Sigorta Poliçesi
            modelBuilder.Entity<SigortaPolicesi>(entity =>
            {
                entity.ToTable("SigortaPoliceleri");
                entity.Property(s => s.SigortaSirketi).HasMaxLength(200).IsRequired();
                entity.Property(s => s.PoliceNo).HasMaxLength(50).IsRequired();
                entity.Property(s => s.Prim).HasPrecision(10, 2);
                entity.HasOne(s => s.Arac).WithMany(a => a.SigortaPoliceleri)
                      .HasForeignKey(s => s.AracId).OnDelete(DeleteBehavior.Cascade);
            });

            // Ceza/İhlal
            modelBuilder.Entity<CezaIhlalKaydi>(entity =>
            {
                entity.ToTable("CezaIhlalKayitlari");
                entity.Property(c => c.CezaTuru).HasMaxLength(200).IsRequired();
                entity.Property(c => c.Tutar).HasPrecision(10, 2);
                entity.HasOne(c => c.Sofor).WithMany(s => s.CezaIhlalKayitlari)
                      .HasForeignKey(c => c.SoforId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(c => c.Arac).WithMany()
                      .HasForeignKey(c => c.AracId).OnDelete(DeleteBehavior.SetNull);
            });

            // Kullanıcı 
            modelBuilder.Entity<Kullanici>(entity =>
            {
                entity.ToTable("Kullanicilar");
                entity.Property(k => k.AdSoyad).HasMaxLength(150).IsRequired();
                entity.Property(k => k.Eposta).HasMaxLength(200).IsRequired();
                entity.HasIndex(k => k.Eposta).IsUnique();
                entity.Property(k => k.SifreHash).HasMaxLength(500).IsRequired();
                entity.HasOne(k => k.Sube).WithMany(s => s.Kullanicilar)
                      .HasForeignKey(k => k.SubeId).OnDelete(DeleteBehavior.SetNull);
            });

            // Şube
            modelBuilder.Entity<Sube>(entity =>
            {
                entity.ToTable("Subeler");
                entity.Property(s => s.Ad).HasMaxLength(200).IsRequired();
                entity.Property(s => s.Sehir).HasMaxLength(100);
            });

            // Masraf 
            modelBuilder.Entity<MasrafKategorisi>(entity =>
            {
                entity.ToTable("MasrafKategorileri");
                entity.Property(m => m.Ad).HasMaxLength(100).IsRequired();
            });

            // Bildirim Şablonu
            modelBuilder.Entity<BildirimSablonu>(entity =>
            {
                entity.ToTable("BildirimSablonlari");
                entity.Property(b => b.Ad).HasMaxLength(200).IsRequired();
                entity.Property(b => b.Konu).HasMaxLength(300).IsRequired();
                entity.Property(b => b.OlayTipi).HasMaxLength(100).IsRequired();
            });
        }
    }
}
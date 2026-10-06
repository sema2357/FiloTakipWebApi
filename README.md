# 🚚 Filo Takip Sistemi

Araç filolarını tek yerden yönetmek için geliştirilmiş web uygulaması. Araç, şoför, sefer, yakıt, bakım ve belge/sigorta süreçlerini takip eder; maliyet ve performans raporları üretir.

## Özellikler

- **Araç yönetimi:** kayıt, detay sayfası, fotoğraf, kilometre güncelleme, muayene/sigorta geçerlilik uyarıları
- **Şoför yönetimi:** kayıt, ehliyet bilgisi ve geçerlilik takibi
- **Sefer yönetimi:** sefer oluşturma, araç/şoför atama, onay akışı
- **Yakıt takibi:** yakıt girişleri, tüketim analizi, anormal tüketim tespiti
- **Bakım:** bakım kayıtları, yaklaşan bakım uyarıları, bakım tamamlama
- **Belge & sigorta:** poliçe ve belge takibi, belge yükleme
- **Raporlar:** filo özeti, araç bazlı maliyet, şoför performansı
- **Kullanıcı & şube yönetimi:** rol tabanlı yetkilendirme (Admin / Yönetici / Kullanıcı)

## Teknolojiler

| Katman | Teknoloji |
|---|---|
| Frontend | React 19, TypeScript, Vite, Tailwind CSS, React Router, Axios |
| Backend | ASP.NET Core Web API (.NET 10), Entity Framework Core, Dapper |
| Veritabanı | Microsoft SQL Server |
| Kimlik doğrulama | JWT, BCrypt ile şifre özetleme |
| Dağıtım | Docker, Docker Compose, nginx |

## Hızlı Başlangıç (Docker)

Gereksinim: [Docker Desktop](https://www.docker.com/products/docker-desktop/)

```bash
# 1. Ortam dosyasını oluşturun
cp .env.example .env        # Windows (PowerShell): Copy-Item .env.example .env

# 2. .env içindeki üç değeri doldurun (aşağıya bakın)

# 3. Başlatın
docker compose up -d --build
```

| Servis | Adres |
|---|---|
| Uygulama (frontend) | http://localhost:5173 |
| API + Swagger | http://localhost:5223 |

İlk girişte `admin@filo.com` e-postası ve `.env` dosyasındaki `ADMIN_SIFRE` değeri kullanılır.

### Ortam değişkenleri (`.env`)

| Değişken | Açıklama |
|---|---|
| `SA_PASSWORD` | SQL Server `sa` şifresi (en az 8 karakter; büyük/küçük harf ve rakam içermeli) |
| `JWT_ANAHTAR` | JWT imzalama anahtarı (en az 32 karakter, rastgele) |
| `ADMIN_SIFRE` | İlk açılışta oluşturulan admin kullanıcısının şifresi |

> **Not:** `.env` dosyası GitHub'a gönderilmez. Gerçek şifreleri asla commit etmeyin.
> `ADMIN_SIFRE` yalnızca veritabanı ilk oluşturulurken kullanılır.

Rastgele değer üretmek için (PowerShell):

```powershell
-join ((48..57)+(65..90)+(97..122) | Get-Random -Count 40 | ForEach-Object {[char]$_})
```

### Faydalı komutlar

```bash
docker compose down          # durdur (veriler kalır)
docker compose up -d         # tekrar başlat
docker compose down -v       # durdur ve tüm verileri sil
docker logs -f filotakip-api # API loglarını izle
```

## Docker olmadan çalıştırma

**Backend** (.NET 10 SDK ve SQL Server / LocalDB gerekir):

```bash
cd FiloTakipWebApi
# PowerShell
$env:Jwt__Anahtar = "en-az-32-karakterlik-rastgele-bir-anahtar"
$env:Admin__Sifre = "guclu-bir-sifre"
dotnet run
```

**Frontend** (Node.js 22+):

```bash
cd FiloTakipFrontend
npm install
npm run dev
```

> Frontend, API adresini [`src/lib/api.ts`](FiloTakipFrontend/src/lib/api.ts) içinde `http://localhost:5223/api` olarak kullanır.

## Proje yapısı

```
├── FiloTakipFrontend/   # React + Vite arayüzü
├── FiloTakipWebApi/     # ASP.NET Core API (Controllers, Services, Models, Migrations)
├── Proje_Taslak/        # Tasarım taslağı (modüller, menü, veri modeli)
├── docker-compose.yml   # SQL Server + API + Frontend
└── .env.example         # Ortam değişkenleri şablonu
```

## Yapılacaklar

- [ ] Masraf modülü (masraf kayıtları ve kategorileri)
- [ ] Arıza kayıtları, yakıt kartları ve lastik takibi
- [ ] Excel / PDF dışa aktarma
- [ ] Ek raporlar (yakıt tüketimi, sefer/km, bakım maliyeti)
- [ ] Otomatik bildirim ve hatırlatmalar

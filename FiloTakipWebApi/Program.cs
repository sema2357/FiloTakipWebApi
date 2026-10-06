using FiloTakipWebApi.Data;
using FiloTakipWebApi.Services;
using FiloTakipWebApi.Models.Entities;
using FiloTakipWebApi.Models.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Veritabanı
builder.Services.AddSingleton<DapperContext>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("FiloTakipVeritabani"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            sqlOptions.CommandTimeout(60);
        }
    )
);

// kimlik doğrulama JWT
var jwtAnahtari = builder.Configuration["Jwt:Anahtar"]
    ?? throw new InvalidOperationException("Jwt:Anahtar ayarı tanımlı değil. Ortam değişkeni (Jwt__Anahtar) ile verin.");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Yayinci"] ?? "FiloTakip",
            ValidAudience = builder.Configuration["Jwt:Izleyici"] ?? "FiloTakip",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtAnahtari)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// Servisler
builder.Services.AddScoped<IKimlikServisi, KimlikServisi>();
builder.Services.AddScoped<IAracServisi, AracServisi>();
builder.Services.AddScoped<IYakitServisi, YakitServisi>();
builder.Services.AddScoped<IBakimServisi, BakimServisi>();
builder.Services.AddScoped<IRaporServisi, RaporServisi>();
builder.Services.AddScoped<IBelgeServisi, BelgeServisi>();
builder.Services.AddScoped<ISeferServisi, SeferServisi>();
builder.Services.AddScoped<ISigortaServisi, SigortaServisi>();
builder.Services.AddScoped<ISoforServisi, SoforServisi>();
builder.Services.AddScoped<ISubeServisi, SubeServisi>();

// cors
builder.Services.AddCors(options =>
    options.AddPolicy("FiloTakipCors", policy =>
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader())
);

builder.Services.AddControllers();

// SWAGGER 
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


var uygulama = builder.Build();
// ---------------------

if (uygulama.Environment.IsDevelopment())
{
    uygulama.UseSwagger();
    uygulama.UseSwaggerUI(seç =>
    {
        seç.SwaggerEndpoint("/swagger/v1/swagger.json", "Filo Takip v1");
        seç.RoutePrefix = string.Empty; // Swagger ana sayfada açılsın
    });
}

uygulama.UseStaticFiles();
uygulama.UseHttpsRedirection();
uygulama.UseCors("FiloTakipCors");
uygulama.UseAuthentication();
uygulama.UseAuthorization();
uygulama.MapControllers();

//data migration
using (var scope = uygulama.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    try
    {
        db.Database.Migrate();
    }
    catch (Exception hata)
    {
        var kayitci = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        kayitci.LogError(hata, "Database migration error.");
    }
    var adminSifre = builder.Configuration["Admin:Sifre"];
    if (!db.Kullanicilar.Any() && !string.IsNullOrWhiteSpace(adminSifre))
    {
        db.Kullanicilar.Add(new Kullanici
        {
            AdSoyad = "Sistem Yöneticisi",
            Eposta = "admin@filo.com",
            SifreHash = BCrypt.Net.BCrypt.HashPassword(adminSifre),
            Rol = KullaniciRolu.Admin,   
            AktifMi = true
        });

        db.SaveChanges();
    }
}

uygulama.Run();

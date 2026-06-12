using FiloTakipWebApi.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Controller Servislerini Ekle
builder.Services.AddControllers();

// 2. Swagger Servislerini Ekle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 3. Veri Tabanı Bağlantısını Ekle
builder.Services.AddDbContext<FiloTakipWebApi.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// 4. Swagger Arayüzünü Geliştirme Ortamında Aktif Et
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Swagger'ın kök dizinde veya düzgün çalışması için endpoint tanımı
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Filo Takip API V1");
    });
}

// Güvenlik ve Yönlendirme Ayarları
app.UseHttpsRedirection();
app.UseAuthorization();

// Controller'ları Haritalandır (API endpointlerini aktif eder)
app.MapControllers();

app.Run();
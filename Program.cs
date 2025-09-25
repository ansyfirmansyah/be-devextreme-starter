using be_devextreme_starter.Controllers;
using be_devextreme_starter.Data;
using be_devextreme_starter.Data.Models;
using be_devextreme_starter.Middleware;
using be_devextreme_starter.Reports;
using be_devextreme_starter.Validators;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// 1. Tambahkan service ke container
// ------------------------------------

// Konfigurasi DbContext untuk EF Core
var baseConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var connectionStringBuilder = new SqlConnectionStringBuilder(baseConnectionString);
connectionStringBuilder.UserID = Environment.GetEnvironmentVariable("DB_USER");
connectionStringBuilder.Password = Environment.GetEnvironmentVariable("DB_PASSWORD");
var fullConnectionString = connectionStringBuilder.ConnectionString;
builder.Services.AddDbContext<DataEntities>(options =>
    options.UseSqlServer(fullConnectionString));

// Konfigurasi DevExtreme
builder.Services.AddDevExpressControls();

// Untuk kebutuhan belajar, set sebagai non commercial
ExcelPackage.License.SetNonCommercialPersonal("Firmansyah");

// Konfigurasi CORS agar React bisa akses API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Ganti dengan URL React App Anda
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});


// Konfigurasi Controller & JSON Serialization
// Tambahkan NewtonsoftJson untuk kompatibilitas penuh dengan DevExtreme
builder.Services
    .AddControllers()
    .AddJsonOptions(options => {
        // Mengatasi circular references jika ada relasi model
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    })
    .AddNewtonsoftJson();

// Daftarkan semua validator
builder.Services.AddValidatorsFromAssemblyContaining<SalesValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PenjualanValidator>();
// Aktifkan validator ke pipeline validasi otomatis
builder.Services.AddFluentValidationAutoValidation(options =>
{
    // Menonaktifkan validasi bawaan (Data Annotations, dll)
    options.DisableDataAnnotationsValidation = true;
});

// Konfigurasi DevExtreme Reporting
builder.Services.AddScoped<IReportProvider, CustomReportProvider>();
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});

// Konfigurasi swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 2. Konfigurasi HTTP request pipeline
// ------------------------------------
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

// Gunakan Middleware Custom
app.UseMiddleware<ErrorHandlerMiddleware>();

app.UseHttpsRedirection();

// Tampilkan Swagger UI hanya di environment Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Gunakan DevExpress Controls (termasuk Reporting)
app.UseDevExpressControls();

app.UseRouting();

// Gunakan CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();

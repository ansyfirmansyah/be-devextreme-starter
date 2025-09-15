var builder = WebApplication.CreateBuilder(args);

// 1. Tambahkan service ke container
// ------------------------------------

// Konfigurasi DbContext untuk EF Core
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Konfigurasi DevExtreme
builder.Services.AddDevExpressControls();

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


// Konfigurasi DevExtreme Reporting
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureReportDesigner(designerConfigurator => {
        designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
    });
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});

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
app.UseStaticFiles();

// Gunakan DevExpress Controls (termasuk Reporting)
app.UseDevExpressControls();

app.UseRouting();

// Gunakan CORS
app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();

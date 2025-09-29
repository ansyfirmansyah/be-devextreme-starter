using be_devextreme_starter.Data.Models;
using be_devextreme_starter.DTOs;
using be_devextreme_starter.Middleware;
using be_devextreme_starter.Reports;
using be_devextreme_starter.Services;
using be_devextreme_starter.Validators;
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Services;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficeOpenXml;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using System.Text.Json.Serialization;

// Perintah ini mengosongkan kamus pemetaan claim internal dari handler JWT.
// Hasilnya, tidak ada lagi penggantian nama otomatis.
// Claim sub akan tetap menjadi sub yang sebelumnya dipetakan menjadi nameidentifier
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

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
                  .AllowAnyMethod()
                  .AllowCredentials(); // Mengizinkan cookie disimpan meskipun berbeda origin
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

// Mengubah format response default dot net (ketika ada invalid model / dto)
// menjadi format response sesuai api response template yang sudah kita buat
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        // Ambil semua pesan error dari ModelState
        var errorMessages = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();

        // Gabungkan semua pesan error menjadi satu string
        var combinedErrorMessage = string.Join(" ", errorMessages);

        // Buat response kustom menggunakan ApiResponse Anda
        var apiResponse = ApiResponse<object>.BadRequest(combinedErrorMessage);

        return new BadRequestObjectResult(apiResponse);
    };
});

// Daftarkan semua validator
builder.Services.AddValidatorsFromAssemblyContaining<SalesValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<PenjualanValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterStep1Validator>();
builder.Services.AddValidatorsFromAssemblyContaining<RegisterStep2Validator>();
builder.Services.AddValidatorsFromAssemblyContaining<LoginValidator>();
builder.Services.AddValidatorsFromAssemblyContaining<ChangePasswordValidator>();
// Aktifkan validator ke pipeline validasi otomatis
builder.Services.AddFluentValidationAutoValidation(options =>
{
    // Menonaktifkan validasi bawaan (Data Annotations, dll)
    options.DisableDataAnnotationsValidation = true;
});

// Menambahkan audit service untuk kebutuhan set created by atau modified by
builder.Services.AddScoped<IAuditService, AuditService>();

// Konfigurasi DevExtreme Reporting
builder.Services.AddScoped<IReportProvider, CustomReportProvider>();
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});

// Konfigurasi swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // Definisikan skema keamanan (JWT Bearer)
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Silakan masukkan token JWT dengan format: Bearer {token}",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    // Terapkan skema keamanan ini secara global ke semua endpoint
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Konfigurasi Autentikasi 
builder.Services.AddAuthentication(options =>
{
    // Skema default jika tidak ada yang spesifik diminta
    options.DefaultScheme = "SmartScheme";
    options.DefaultChallengeScheme = "SmartScheme";
})
.AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.Name = "be-devextreme-starter"; // nama cookie disesuaikan agar tidak conflict
    options.Cookie.HttpOnly = true;
    if (builder.Environment.IsDevelopment())
    {
        // Di development, izinkan cookie dikirim dari http (React dev server) ke https (backend)
        options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
    }
    else
    {
        // Di produksi, WAJIB Always
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    }
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
    // Redirect non-API request ke halaman login (jika ada halaman MVC/Razor)
    // Untuk API, kita akan atur agar mengembalikan 401 Unauthorized
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
})
.AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Konfigurasi JWT Anda yang sudah ada, tidak berubah
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),

        // ASP.NET Core secara default memberikan "masa tenggang" atau ClockSkew selama 5 menit
        // baris ini untuk menonaktifkan masa tenggang tersebut / membuatnya menjadi 0 menit
        ClockSkew = TimeSpan.Zero
    };
})
// Ini adalah "skema pintar" yang akan memilih antara Cookie atau JWT
.AddPolicyScheme("SmartScheme", "SmartScheme", options =>
{
    options.ForwardDefaultSelector = context =>
    {
        // Jika request memiliki header "Authorization" dengan "Bearer", gunakan skema JWT
        string authorization = context.Request.Headers["Authorization"];
        if (!string.IsNullOrEmpty(authorization) && authorization.StartsWith("Bearer "))
        {
            return JwtBearerDefaults.AuthenticationScheme;
        }

        // Jika tidak, asumsikan ini adalah browser dan gunakan skema Cookie
        return CookieAuthenticationDefaults.AuthenticationScheme;
    };
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

// PENTING: Tambahkan ini SETELAH .UseRouting() dan SEBELUM .UseAuthorization()
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();

using Microsoft.EntityFrameworkCore;
using LegendCraft_Backend.Data;
using LegendCraft_Backend.Services;
using Microsoft.Extensions.FileProviders;
using System.IO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using LegendCraft_Backend.Models;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Base de Datos
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// CONFIGURACIÓN DE IDENTITY
// Esto conecta las clases de usuario de Microsoft con tu base de datos PostgreSQL
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<LegendCraft_Backend.Helpers.CustomIdentityErrorDescriber>();

// CONFIGURACIÓN DE JWT
// Leemos la clave secreta desde el appsettings.json (o secrets.json)
var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];

builder.Services.AddAuthentication(options =>
{
    // Le decimos a .NET que usaremos JWT por defecto para autenticar
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Configuramos cómo .NET debe validar los tokens entrantes
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!))
    };
});

// Inyección de dependencias de tus servicios
builder.Services.AddScoped<IImageStorageService, LocalImageStorageService>();
builder.Services.AddScoped<IArticleService, ArticleService>();
builder.Services.AddScoped<IAttributeService, AttributeService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IBannerService, BannerService>();
builder.Services.AddScoped<IFaqService, FaqService>();

// CORS para permitir peticiones del frontend (ej. React, Angular, Vue)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();

// APLICAR POLÍTICA CORS (Antes de Autenticación)
app.UseCors("AllowAll");

// ACTIVAR LA AUTENTICACIÓN
// Es vital que UseAuthentication esté ANTES de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

// Definimos la ruta física en el servidor/contenedor
var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
if (!Directory.Exists(uploadsPath))
{
    Directory.CreateDirectory(uploadsPath);
}

// Habilitamos que .NET sirva archivos desde esa carpeta física
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(uploadsPath),
    RequestPath = "/imagenes"
});

app.MapControllers();
app.Run();
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OfficinaGestionale.Api.Context;
using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Models;
using OfficinaGestionale.Api.Repositories;
using OfficinaGestionale.Api.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// dati JWT letti da configurazione
string jwtKey = builder.Configuration["Jwt:Key"]!;
string jwtIssuer = builder.Configuration["Jwt:Issuer"]!;
string jwtAudience = builder.Configuration["Jwt:Audience"]!;

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Inserisci il JWT così: Bearer {token}"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddDbContext<OfficinaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Database")));

// TODO: in produzione limitare ai domini del frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("Open", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// repository e servizi applicativi
builder.Services.AddScoped<IRepoLettura<Cliente>, ClienteRepo>();
builder.Services.AddScoped<IRepoScrittura<Cliente>, ClienteRepo>();
builder.Services.AddScoped<IVeicoloRepo, VeicoloRepo>();
builder.Services.AddScoped<IRepoLettura<Intervento>, InterventoRepo>();
builder.Services.AddScoped<IRepoScrittura<Intervento>, InterventoRepo>();

builder.Services.AddScoped<VeicoloService>();
builder.Services.AddScoped<IService<ClienteDto>, ClienteService>();
builder.Services.AddScoped<IService<VeicoloDto>, VeicoloService>();
builder.Services.AddScoped<IService<InterventoDto>, InterventoService>();

builder.Services.AddScoped<IRepoLettura<Preventivo>, PreventivoRepo>();
builder.Services.AddScoped<IRepoScrittura<Preventivo>, PreventivoRepo>();
builder.Services.AddScoped<IRepoLettura<Fattura>, FatturaRepo>();
builder.Services.AddScoped<IRepoScrittura<Fattura>, FatturaRepo>();
builder.Services.AddScoped<FatturaService>();
builder.Services.AddScoped<IService<PreventivoDto>, PreventivoService>();
builder.Services.AddScoped<IService<FatturaDto>, FatturaService>();

WebApplication app = builder.Build();

using (IServiceScope scope = app.Services.CreateScope())
{
    OfficinaContext db = scope.ServiceProvider.GetRequiredService<OfficinaContext>();

    // applico le migration all'avvio in ambiente di sviluppo
    db.Database.Migrate();

    // utente admin di default per i test locali
    if (!db.Utenti.Any())
    {
        byte[] salt = RandomNumberGenerator.GetBytes(16);
        byte[] hash = Rfc2898DeriveBytes.Pbkdf2("Admin123!", salt, 100_000, HashAlgorithmName.SHA256, 32);
        db.Utenti.Add(new Utente
        {
            Email = "admin@officina.it",
            Nome = "Amministratore",
            Ruolo = "Admin",
            PasswordHash = $"{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}"
        });
        db.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseCors("Open");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
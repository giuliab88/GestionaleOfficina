using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using OfficinaGestionale.Api.Context;
using OfficinaGestionale.Api.DTOs;

namespace OfficinaGestionale.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly OfficinaContext _db;
    private readonly IConfiguration _config;

    public AuthController(OfficinaContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    // controllo email e password, poi restituisco il token JWT
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequestDto dto)
    {
        var utente = _db.Utenti.FirstOrDefault(u => u.Email == dto.Email);
        if (utente is null || !VerificaPassword(dto.Password, utente.PasswordHash))
            return Unauthorized("Credenziali non valide.");

        string token = GeneraToken(utente.UtenteId, utente.Email);
        return Ok(new { token, nome = utente.Nome, ruolo = utente.Ruolo });
    }

    private string GeneraToken(int userId, string email)
    {
        // dati letti da configurazione: in dev da appsettings.Development, in produzione da env/secrets
        string key      = _config["Jwt:Key"]!;
        string issuer   = _config["Jwt:Issuer"]!;
        string audience = _config["Jwt:Audience"]!;

        SymmetricSecurityKey secKey = new(Encoding.UTF8.GetBytes(key));
        SigningCredentials creds    = new(secKey, SecurityAlgorithms.HmacSha256);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub,        userId.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, email),
            new(JwtRegisteredClaimNames.Jti,        Guid.NewGuid().ToString()),
        ];

        JwtSecurityToken token = new(
            issuer:            issuer,
            audience:          audience,
            claims:            claims,
            expires:           DateTime.UtcNow.AddHours(8),// durata sessione
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static bool VerificaPassword(string password, string hashSalvato)
    {
        // formato salvato: salt.hash
        string[] parti = hashSalvato.Split('.');
        if (parti.Length != 2) return false;
        byte[] salt          = Convert.FromBase64String(parti[0]);
        byte[] hashAtteso    = Convert.FromBase64String(parti[1]);
        byte[] hashCalcolato = Rfc2898DeriveBytes.Pbkdf2(password, salt, 100_000, HashAlgorithmName.SHA256, 32);
        // confronto sicuro contro timing attack
        return CryptographicOperations.FixedTimeEquals(hashCalcolato, hashAtteso);
    }
}

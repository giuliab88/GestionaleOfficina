namespace OfficinaGestionale.Api.Models;

public class Utente
{
    public int UtenteId { get; set; }
    public string Email { get; set; } = "";
    // formato: base64(salt).base64(hash) — generato con PBKDF2/SHA256
    public string PasswordHash { get; set; } = "";
    public string Nome { get; set; } = "";
    public string Ruolo { get; set; } = "Operatore";
}

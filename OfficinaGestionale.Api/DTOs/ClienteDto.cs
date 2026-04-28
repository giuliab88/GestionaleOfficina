namespace OfficinaGestionale.Api.DTOs;

public class ClienteDto
{
    public string Codice { get; set; } = string.Empty;
    public string Nome { get; set; } = string.Empty;
    public string Cognome { get; set; } = string.Empty;
    public string? Indirizzo { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? CodiceFiscale { get; set; }
    public string? PartitaIva { get; set; }
}

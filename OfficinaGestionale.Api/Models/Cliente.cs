using System.ComponentModel.DataAnnotations;

namespace OfficinaGestionale.Api.Models;

public class Cliente
{
    public int ClienteId { get; set; }

    [Required]
    public string Codice { get; set; } = string.Empty;

    [Required]
    public string Nome { get; set; } = string.Empty;

    [Required]
    public string Cognome { get; set; } = string.Empty;

    public string? Indirizzo { get; set; }
    public string? Telefono { get; set; }
    public string? Email { get; set; }
    public string? CodiceFiscale { get; set; }
    public string? PartitaIva { get; set; }

    public ICollection<Veicolo> Veicoli { get; set; } = new List<Veicolo>();
}

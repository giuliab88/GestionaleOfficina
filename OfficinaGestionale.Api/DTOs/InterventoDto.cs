using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.DTOs;

public class InterventoDto
{
    public string Codice { get; set; } = string.Empty;
    public string? Descrizione { get; set; }
    public decimal Prezzo { get; set; }
    public StatoIntervento Stato { get; set; } = StatoIntervento.Aperto;
    public DateTime DataIngresso { get; set; }
    public string TargaVeicolo { get; set; } = string.Empty;
    public VeicoloDto? Veicolo { get; set; }
}

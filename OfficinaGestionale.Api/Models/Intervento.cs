using System.ComponentModel.DataAnnotations;

namespace OfficinaGestionale.Api.Models;

public class Intervento
{
    public int InterventoId { get; set; }

    [Required]
    public string Codice { get; set; } = string.Empty;

    public string? Descrizione { get; set; }

    public decimal Prezzo { get; set; }

    public StatoIntervento Stato { get; set; } = StatoIntervento.Aperto;

    // mappato su data_ingresso nel db per via di una migrazione vecchia
    public DateTime DataIngresso { get; set; }

    public int VeicoloRif { get; set; }
    public Veicolo? Veicolo { get; set; }
}

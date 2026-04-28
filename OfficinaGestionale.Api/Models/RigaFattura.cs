using System.ComponentModel.DataAnnotations;

namespace OfficinaGestionale.Api.Models;

public class RigaFattura
{
    public int RigaFatturaId { get; set; }

    [Required]
    public string Descrizione { get; set; } = string.Empty;

    public decimal Quantita { get; set; } = 1;

    public decimal PrezzoUnitario { get; set; }

    public int FatturaRif { get; set; }
    public Fattura? Fattura { get; set; }
}

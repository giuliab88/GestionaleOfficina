using System.ComponentModel.DataAnnotations;

namespace OfficinaGestionale.Api.Models;

public class VocePreventivo
{
    public int VocePreventivoId { get; set; }

    [Required]
    public string Descrizione { get; set; } = string.Empty;

    public decimal Quantita { get; set; } = 1;

    public decimal PrezzoUnitario { get; set; }

    public int PreventivoRif { get; set; }
    public Preventivo? Preventivo { get; set; }
}

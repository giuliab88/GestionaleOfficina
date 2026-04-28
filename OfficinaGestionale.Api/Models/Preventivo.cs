using System.ComponentModel.DataAnnotations;

namespace OfficinaGestionale.Api.Models;

public class Preventivo
{
    public int PreventivoId { get; set; }

    [Required]
    public string Codice { get; set; } = string.Empty;

    public DateTime Data { get; set; }

    public string? Note { get; set; }

    public StatoPreventivo Stato { get; set; } = StatoPreventivo.Bozza;

    public decimal AliquotaIva { get; set; } = 22m;
    public DateTime? ValidoFino { get; set; }

    public int VeicoloRif { get; set; }
    public Veicolo? Veicolo { get; set; }

    public List<VocePreventivo> Voci { get; set; } = new();
}

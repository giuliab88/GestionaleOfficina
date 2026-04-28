using System.ComponentModel.DataAnnotations;

namespace OfficinaGestionale.Api.Models;

public class Fattura
{
    public int FatturaId { get; set; }

    [Required]
    public string Codice { get; set; } = string.Empty;

    public DateTime DataEmissione { get; set; }

    public DateTime? DataScadenza { get; set; }

    public string? Note { get; set; }

    public StatoFattura Stato { get; set; } = StatoFattura.Emessa;

    // aliquota IVA in percentuale (es. 22 = 22%) — default ordinario italiano
    public decimal AliquotaIva { get; set; } = 22m;
    public string? ModalitaPagamento { get; set; }

    public int VeicoloRif { get; set; }
    public Veicolo? Veicolo { get; set; }

    public int? PreventivoRif { get; set; }
    public Preventivo? Preventivo { get; set; }

    public List<RigaFattura> Righe { get; set; } = new();
}

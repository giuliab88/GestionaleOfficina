using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.DTOs;

public class FatturaDto
{
    public string Codice { get; set; } = string.Empty;
    public DateTime DataEmissione { get; set; }
    public DateTime? DataScadenza { get; set; }
    public string? Note { get; set; }
    public StatoFattura Stato { get; set; } = StatoFattura.Emessa;
    public decimal AliquotaIva { get; set; } = 22m;
    public string? ModalitaPagamento { get; set; }
    public string TargaVeicolo { get; set; } = string.Empty;
    public VeicoloDto? Veicolo { get; set; }
    public string? CodicePreventivo { get; set; }
    public List<RigaFatturaDto> Righe { get; set; } = new();

    public decimal Imponibile => Righe.Sum(r => r.Totale);
    public decimal ImportoIva => Math.Round(Imponibile * AliquotaIva / 100m, 2);
    public decimal Totale => Imponibile + ImportoIva;
}

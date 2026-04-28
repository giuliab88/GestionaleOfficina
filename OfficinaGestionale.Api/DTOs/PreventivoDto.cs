using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.DTOs;

public class PreventivoDto
{
    public string Codice { get; set; } = string.Empty;
    public DateTime Data { get; set; }
    public DateTime? ValidoFino { get; set; }
    public string? Note { get; set; }
    public StatoPreventivo Stato { get; set; } = StatoPreventivo.Bozza;
    public decimal AliquotaIva { get; set; } = 22m;
    public string TargaVeicolo { get; set; } = string.Empty;
    public VeicoloDto? Veicolo { get; set; }
    public List<VocePreventivoDto> Voci { get; set; } = new();

    public decimal Imponibile => Voci.Sum(v => v.Totale);
    public decimal ImportoIva => Math.Round(Imponibile * AliquotaIva / 100m, 2);
    public decimal Totale => Imponibile + ImportoIva;
}

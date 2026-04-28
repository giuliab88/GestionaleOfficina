namespace OfficinaGestionale.Api.DTOs;

public class RigaFatturaDto
{
    public int? RigaFatturaId { get; set; }
    public string Descrizione { get; set; } = string.Empty;
    public decimal Quantita { get; set; } = 1;
    public decimal PrezzoUnitario { get; set; }
    public decimal Totale => Quantita * PrezzoUnitario;
}

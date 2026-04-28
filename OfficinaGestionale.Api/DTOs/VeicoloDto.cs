namespace OfficinaGestionale.Api.DTOs;

public class VeicoloDto
{
    public string Codice { get; set; } = string.Empty;
    public string Targa { get; set; } = string.Empty;
    public string Marca { get; set; } = string.Empty;
    public string Modello { get; set; } = string.Empty;
    public int Anno { get; set; }
    public string CodiceCliente { get; set; } = string.Empty;
    // dati anagrafici del cliente — utili per mostrare il nome in fattura
    public string NomeCliente { get; set; } = string.Empty;
    public string CognomeCliente { get; set; } = string.Empty;
}

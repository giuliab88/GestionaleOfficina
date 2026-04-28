using System.ComponentModel.DataAnnotations;

namespace OfficinaGestionale.Api.Models;

public class Veicolo
{
    public int VeicoloId { get; set; }

    [Required]
    public string Codice { get; set; } = string.Empty;

    [Required]
    public string Targa { get; set; } = string.Empty;

    [Required]
    public string Marca { get; set; } = string.Empty;

    [Required]
    public string Modello { get; set; } = string.Empty;

    public int Anno { get; set; }

    public int ClienteRif { get; set; }
    public Cliente? Cliente { get; set; }

    public ICollection<Intervento> Interventi { get; set; } = new List<Intervento>();
}

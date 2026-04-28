using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Models;
using OfficinaGestionale.Api.Repositories;

namespace OfficinaGestionale.Api.Services;

public class InterventoService : IService<InterventoDto>
{
    private readonly IRepoLettura<Intervento> _repoLettura;
    private readonly IRepoScrittura<Intervento> _repoScrittura;
    private readonly VeicoloService _veicoloService;

    public InterventoService(
        IRepoLettura<Intervento> repoLettura,
        IRepoScrittura<Intervento> repoScrittura,
        VeicoloService veicoloService)
    {
        _repoLettura = repoLettura;
        _repoScrittura = repoScrittura;
        _veicoloService = veicoloService;
    }

    public InterventoDto? CercaPerCodice(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return null;

        Intervento? intervento = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        return intervento is null ? null : Map(intervento);
    }

    public IEnumerable<InterventoDto> CercaTutti()
    {
        return _repoLettura.GetAll().Select(Map).ToList();
    }

    public ServiceResult Inserisci(InterventoDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.TargaVeicolo))
            return ServiceResult.Fail("Il veicolo di riferimento è obbligatorio.");

        int? veicoloId = _veicoloService.CercaVeicoloIdPerTarga(entity.TargaVeicolo.Trim().ToUpper());
        if (veicoloId is null)
            return ServiceResult.NotFound("Veicolo non trovato.");

        // se non arriva un codice lo genero automaticamente
        string codice = string.IsNullOrWhiteSpace(entity.Codice)
            ? GeneraCodice()
            : entity.Codice.Trim().ToUpper();

        if (_repoLettura.GetByCodice(codice) is not null)
            return ServiceResult.Conflict("Codice già in uso.");

        entity.Codice = codice;

        Intervento intervento = new()
        {
            Codice = codice,
            Descrizione = entity.Descrizione?.Trim(),
            Prezzo = entity.Prezzo,
            Stato = entity.Stato,
            DataIngresso = entity.DataIngresso == default ? DateTime.Today : entity.DataIngresso.Date,
            VeicoloRif = veicoloId.Value
        };

        return _repoScrittura.Create(intervento)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante il salvataggio.");
    }

    public ServiceResult Aggiorna(InterventoDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        Intervento? intervento = _repoLettura.GetByCodice(entity.Codice.Trim().ToUpper());
        if (intervento is null)
            return ServiceResult.NotFound("Intervento non trovato.");

        if (!string.IsNullOrWhiteSpace(entity.TargaVeicolo))
        {
            int? veicoloId = _veicoloService.CercaVeicoloIdPerTarga(entity.TargaVeicolo.Trim().ToUpper());
            if (veicoloId is null)
                return ServiceResult.NotFound("Veicolo non trovato.");
            intervento.VeicoloRif = veicoloId.Value;
        }

        if (!string.IsNullOrWhiteSpace(entity.Descrizione))
            intervento.Descrizione = entity.Descrizione.Trim();

        intervento.Prezzo = entity.Prezzo;
        intervento.Stato = entity.Stato;

        if (entity.DataIngresso != default)
            intervento.DataIngresso = entity.DataIngresso.Date;

        return _repoScrittura.Update(intervento)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'aggiornamento.");
    }

    public ServiceResult Elimina(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        Intervento? intervento = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        if (intervento is null)
            return ServiceResult.NotFound("Intervento non trovato.");

        return _repoScrittura.Delete(intervento.InterventoId)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'eliminazione.");
    }

    // genera codice progressivo tipo INT2026-001
    private string GeneraCodice()
    {
        int anno = DateTime.Today.Year;
        string pref = $"INT{anno}-";
        int max = _repoLettura.GetAll()
            .Select(i => i.Codice)
            .Where(c => c.StartsWith(pref))
            .Select(c => int.TryParse(c[pref.Length..], out int n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"{pref}{(max + 1):D3}";
    }

    // mapping entity -- dto
    private static InterventoDto Map(Intervento intervento) => new()
    {
        Codice = intervento.Codice,
        Descrizione = intervento.Descrizione,
        Prezzo = intervento.Prezzo,
        Stato = intervento.Stato,
        DataIngresso = intervento.DataIngresso,
        TargaVeicolo = intervento.Veicolo?.Targa ?? string.Empty,
        Veicolo = intervento.Veicolo is null ? null : new VeicoloDto
        {
            Targa = intervento.Veicolo.Targa,
            Marca = intervento.Veicolo.Marca,
            Modello = intervento.Veicolo.Modello,
            Anno = intervento.Veicolo.Anno,
            CodiceCliente = intervento.Veicolo.Cliente?.Codice ?? string.Empty
        }
    };
}

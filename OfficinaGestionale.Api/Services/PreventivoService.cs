using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Models;
using OfficinaGestionale.Api.Repositories;

namespace OfficinaGestionale.Api.Services;

public class PreventivoService : IService<PreventivoDto>
{
    private readonly IRepoLettura<Preventivo> _repoLettura;
    private readonly IRepoScrittura<Preventivo> _repoScrittura;
    private readonly VeicoloService _veicoloService;

    public PreventivoService(
        IRepoLettura<Preventivo> repoLettura,
        IRepoScrittura<Preventivo> repoScrittura,
        VeicoloService veicoloService)
    {
        _repoLettura = repoLettura;
        _repoScrittura = repoScrittura;
        _veicoloService = veicoloService;
    }

    public PreventivoDto? CercaPerCodice(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return null;

        Preventivo? p = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        return p is null ? null : Map(p);
    }

    public IEnumerable<PreventivoDto> CercaTutti()
    {
        return _repoLettura.GetAll().Select(Map);
    }

    public ServiceResult Inserisci(PreventivoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TargaVeicolo))
            return ServiceResult.Fail("Il veicolo di riferimento è obbligatorio.");

        if (dto.Voci.Count == 0)
            return ServiceResult.Fail("Inserire almeno una voce.");

        if (dto.Voci.Any(v => string.IsNullOrWhiteSpace(v.Descrizione) || v.Quantita <= 0 || v.PrezzoUnitario < 0))
            return ServiceResult.Fail("Le voci del preventivo non sono valide.");

        int? veicoloId = _veicoloService.CercaVeicoloIdPerTarga(dto.TargaVeicolo.Trim().ToUpper());
        if (veicoloId is null)
            return ServiceResult.NotFound("Veicolo non trovato.");

        // se non arriva un codice lo genero automaticamente
        string codice = string.IsNullOrWhiteSpace(dto.Codice)
            ? GeneraCodice()
            : dto.Codice.Trim().ToUpper();

        if (_repoLettura.GetByCodice(codice) is not null)
            return ServiceResult.Conflict("Codice già in uso.");

        dto.Codice = codice;

        Preventivo preventivo = new()
        {
            Codice = codice,
            Data = dto.Data == default ? DateTime.Today : dto.Data.Date,
            ValidoFino = dto.ValidoFino?.Date,
            Note = dto.Note?.Trim(),
            Stato = dto.Stato,
            AliquotaIva = dto.AliquotaIva,
            VeicoloRif = veicoloId.Value,
            Voci = dto.Voci.Select(v => new VocePreventivo
            {
                Descrizione = v.Descrizione.Trim(),
                Quantita = v.Quantita,
                PrezzoUnitario = v.PrezzoUnitario
            }).ToList()
        };

        return _repoScrittura.Create(preventivo)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante il salvataggio.");
    }

    public ServiceResult Aggiorna(PreventivoDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        if (dto.Voci.Count == 0)
            return ServiceResult.Fail("Inserire almeno una voce.");

        if (dto.Voci.Any(v => string.IsNullOrWhiteSpace(v.Descrizione) || v.Quantita <= 0 || v.PrezzoUnitario < 0))
            return ServiceResult.Fail("Le voci del preventivo non sono valide.");

        Preventivo? esistente = _repoLettura.GetByCodice(dto.Codice.Trim().ToUpper());
        if (esistente is null)
            return ServiceResult.NotFound("Preventivo non trovato.");

        if (!string.IsNullOrWhiteSpace(dto.TargaVeicolo))
        {
            int? veicoloId = _veicoloService.CercaVeicoloIdPerTarga(dto.TargaVeicolo.Trim().ToUpper());
            if (veicoloId is null)
                return ServiceResult.NotFound("Veicolo non trovato.");
            esistente.VeicoloRif = veicoloId.Value;
        }

        esistente.Data = dto.Data == default ? esistente.Data : dto.Data.Date;
        esistente.ValidoFino = dto.ValidoFino?.Date;
        esistente.Note = dto.Note?.Trim();
        esistente.Stato = dto.Stato;
        esistente.AliquotaIva = dto.AliquotaIva;

        // sostituisco le voci con quelle arrivate dal form
        esistente.Voci = dto.Voci.Select(v => new VocePreventivo
        {
            Descrizione = v.Descrizione.Trim(),
            Quantita = v.Quantita,
            PrezzoUnitario = v.PrezzoUnitario
        }).ToList();

        return _repoScrittura.Update(esistente)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'aggiornamento.");
    }

    public ServiceResult Elimina(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        Preventivo? p = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        if (p is null)
            return ServiceResult.NotFound("Preventivo non trovato.");

        return _repoScrittura.Delete(p.PreventivoId)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'eliminazione.");
    }

    // genera codice progressivo tipo PREV2026-001
    private string GeneraCodice()
    {
        int anno = DateTime.Today.Year;
        string pref = $"PREV{anno}-";
        int max = _repoLettura.GetAll()
            .Select(p => p.Codice)
            .Where(c => c.StartsWith(pref))
            .Select(c => int.TryParse(c[pref.Length..], out int n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"{pref}{(max + 1):D3}";
    }

    // mapping entity -- dto
    private static PreventivoDto Map(Preventivo p) => new()
    {
        Codice = p.Codice,
        Data = p.Data,
        ValidoFino = p.ValidoFino,
        Note = p.Note,
        Stato = p.Stato,
        AliquotaIva = p.AliquotaIva,
        TargaVeicolo = p.Veicolo?.Targa ?? string.Empty,
        Veicolo = p.Veicolo is null ? null : new VeicoloDto
        {
            Targa = p.Veicolo.Targa,
            Marca = p.Veicolo.Marca,
            Modello = p.Veicolo.Modello,
            Anno = p.Veicolo.Anno,
            CodiceCliente = p.Veicolo.Cliente?.Codice ?? string.Empty,
            NomeCliente = p.Veicolo.Cliente?.Nome ?? string.Empty,
            CognomeCliente = p.Veicolo.Cliente?.Cognome ?? string.Empty
        },
        Voci = p.Voci.Select(v => new VocePreventivoDto
        {
            VocePreventivoId = v.VocePreventivoId,
            Descrizione = v.Descrizione,
            Quantita = v.Quantita,
            PrezzoUnitario = v.PrezzoUnitario
        }).ToList()
    };
}
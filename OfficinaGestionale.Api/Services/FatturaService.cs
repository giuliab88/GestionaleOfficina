using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Models;
using OfficinaGestionale.Api.Repositories;

namespace OfficinaGestionale.Api.Services;

public class FatturaService : IService<FatturaDto>
{
    private readonly IRepoLettura<Fattura> _repoLettura;
    private readonly IRepoScrittura<Fattura> _repoScrittura;
    private readonly IRepoLettura<Preventivo> _prevLettura;
    private readonly IRepoScrittura<Preventivo> _prevScrittura;
    private readonly VeicoloService _veicoloService;

    public FatturaService(
        IRepoLettura<Fattura> repoLettura,
        IRepoScrittura<Fattura> repoScrittura,
        IRepoLettura<Preventivo> prevLettura,
        IRepoScrittura<Preventivo> prevScrittura,
        VeicoloService veicoloService)
    {
        _repoLettura = repoLettura;
        _repoScrittura = repoScrittura;
        _prevLettura = prevLettura;
        _prevScrittura = prevScrittura;
        _veicoloService = veicoloService;
    }

    public FatturaDto? CercaPerCodice(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return null;

        Fattura? f = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        return f is null ? null : Map(f);
    }

    public IEnumerable<FatturaDto> CercaTutti()
    {
        return _repoLettura.GetAll().Select(Map);
    }

    public ServiceResult Inserisci(FatturaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.TargaVeicolo))
            return ServiceResult.Fail("Il veicolo di riferimento è obbligatorio.");

        if (dto.Righe.Count == 0)
            return ServiceResult.Fail("Inserire almeno una riga.");

        if (dto.Righe.Any(r => string.IsNullOrWhiteSpace(r.Descrizione) || r.Quantita <= 0 || r.PrezzoUnitario < 0))
            return ServiceResult.Fail("Le righe della fattura non sono valide.");

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

        int? prevId = null;
        if (!string.IsNullOrWhiteSpace(dto.CodicePreventivo))
        {
            Preventivo? prev = _prevLettura.GetByCodice(dto.CodicePreventivo.Trim().ToUpper());
            if (prev is null)
                return ServiceResult.NotFound("Preventivo di riferimento non trovato.");
            prevId = prev.PreventivoId;
        }

        Fattura fattura = new()
        {
            Codice = codice,
            DataEmissione = dto.DataEmissione == default ? DateTime.Today : dto.DataEmissione.Date,
            DataScadenza = dto.DataScadenza?.Date,
            Note = dto.Note?.Trim(),
            Stato = dto.Stato,
            AliquotaIva = dto.AliquotaIva,
            ModalitaPagamento = dto.ModalitaPagamento?.Trim(),
            VeicoloRif = veicoloId.Value,
            PreventivoRif = prevId,
            Righe = dto.Righe.Select(r => new RigaFattura
            {
                Descrizione = r.Descrizione.Trim(),
                Quantita = r.Quantita,
                PrezzoUnitario = r.PrezzoUnitario
            }).ToList()
        };

        return _repoScrittura.Create(fattura)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante il salvataggio.");
    }

    public ServiceResult Aggiorna(FatturaDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        if (dto.Righe.Count == 0)
            return ServiceResult.Fail("Inserire almeno una riga.");

        if (dto.Righe.Any(r => string.IsNullOrWhiteSpace(r.Descrizione) || r.Quantita <= 0 || r.PrezzoUnitario < 0))
            return ServiceResult.Fail("Le righe della fattura non sono valide.");

        Fattura? esistente = _repoLettura.GetByCodice(dto.Codice.Trim().ToUpper());
        if (esistente is null)
            return ServiceResult.NotFound("Fattura non trovata.");

        if (!string.IsNullOrWhiteSpace(dto.TargaVeicolo))
        {
            int? veicoloId = _veicoloService.CercaVeicoloIdPerTarga(dto.TargaVeicolo.Trim().ToUpper());
            if (veicoloId is null)
                return ServiceResult.NotFound("Veicolo non trovato.");
            esistente.VeicoloRif = veicoloId.Value;
        }

        esistente.DataEmissione = dto.DataEmissione == default ? esistente.DataEmissione : dto.DataEmissione.Date;
        esistente.DataScadenza = dto.DataScadenza?.Date;
        esistente.Note = dto.Note?.Trim();
        esistente.Stato = dto.Stato;
        esistente.AliquotaIva = dto.AliquotaIva;
        esistente.ModalitaPagamento = dto.ModalitaPagamento?.Trim();

        // sostituisco le righe con quelle arrivate dal form
        esistente.Righe = dto.Righe.Select(r => new RigaFattura
        {
            Descrizione = r.Descrizione.Trim(),
            Quantita = r.Quantita,
            PrezzoUnitario = r.PrezzoUnitario
        }).ToList();

        return _repoScrittura.Update(esistente)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'aggiornamento.");
    }

    public ServiceResult Elimina(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        Fattura? f = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        if (f is null)
            return ServiceResult.NotFound("Fattura non trovata.");

        return _repoScrittura.Delete(f.FatturaId)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'eliminazione.");
    }

    public ServiceResult<FatturaDto> ConvertitoDaPreventivo(string codicePreventivo)
    {
        if (string.IsNullOrWhiteSpace(codicePreventivo))
            return ServiceResult<FatturaDto>.Fail("Codice preventivo obbligatorio.");

        Preventivo? prev = _prevLettura.GetByCodice(codicePreventivo.Trim().ToUpper());
        if (prev is null)
            return ServiceResult<FatturaDto>.NotFound("Preventivo non trovato.");

        // evito di creare due fatture dallo stesso preventivo
        if (_repoLettura.GetAll().Any(f => f.PreventivoRif == prev.PreventivoId))
            return ServiceResult<FatturaDto>.Conflict("Esiste già una fattura per questo preventivo.");

        string codice = GeneraCodice();

        // copio i dati principali del preventivo nella nuova fattura
        Fattura fattura = new()
        {
            Codice = codice,
            DataEmissione = DateTime.Today,
            Stato = StatoFattura.Emessa,
            AliquotaIva = prev.AliquotaIva,
            VeicoloRif = prev.VeicoloRif,
            PreventivoRif = prev.PreventivoId,
            Note = prev.Note,
            Righe = prev.Voci.Select(v => new RigaFattura
            {
                Descrizione = v.Descrizione,
                Quantita = v.Quantita,
                PrezzoUnitario = v.PrezzoUnitario
            }).ToList()
        };

        if (!_repoScrittura.Create(fattura))
            return ServiceResult<FatturaDto>.ServerError("Errore durante il salvataggio.");

        // segno il preventivo come accettato dopo la conversione
        prev.Stato = StatoPreventivo.Accettato;
        _prevScrittura.Update(prev);

        FatturaDto? dto = CercaPerCodice(codice);
        return dto is not null
            ? ServiceResult<FatturaDto>.Ok(dto)
            : ServiceResult<FatturaDto>.ServerError("Fattura creata ma non recuperata.");
    }

    // genera codice progressivo tipo FAT2026-001
    private string GeneraCodice()
    {
        int anno = DateTime.Today.Year;
        string pref = $"FAT{anno}-";
        int max = _repoLettura.GetAll()
            .Select(f => f.Codice)
            .Where(c => c.StartsWith(pref))
            .Select(c => int.TryParse(c[pref.Length..], out int n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"{pref}{(max + 1):D3}";
    }

    // mapping entity -- dto
    private static FatturaDto Map(Fattura f) => new()
    {
        Codice = f.Codice,
        DataEmissione = f.DataEmissione,
        DataScadenza = f.DataScadenza,
        Note = f.Note,
        Stato = f.Stato,
        AliquotaIva = f.AliquotaIva,
        ModalitaPagamento = f.ModalitaPagamento,
        TargaVeicolo = f.Veicolo?.Targa ?? string.Empty,
        CodicePreventivo = f.Preventivo?.Codice,
        Veicolo = f.Veicolo is null ? null : new VeicoloDto
        {
            Targa = f.Veicolo.Targa,
            Marca = f.Veicolo.Marca,
            Modello = f.Veicolo.Modello,
            Anno = f.Veicolo.Anno,
            CodiceCliente = f.Veicolo.Cliente?.Codice ?? string.Empty,
            NomeCliente = f.Veicolo.Cliente?.Nome ?? string.Empty,
            CognomeCliente = f.Veicolo.Cliente?.Cognome ?? string.Empty
        },
        Righe = f.Righe.Select(r => new RigaFatturaDto
        {
            RigaFatturaId = r.RigaFatturaId,
            Descrizione = r.Descrizione,
            Quantita = r.Quantita,
            PrezzoUnitario = r.PrezzoUnitario
        }).ToList()
    };
}
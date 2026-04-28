using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Models;
using OfficinaGestionale.Api.Repositories;

namespace OfficinaGestionale.Api.Services;

public class VeicoloService : IService<VeicoloDto>
{
    private readonly IVeicoloRepo _repoVeicolo;
    private readonly IRepoLettura<Cliente> _repoCliente;

    public VeicoloService(
        IVeicoloRepo repoVeicolo,
        IRepoLettura<Cliente> repoCliente)
    {
        _repoVeicolo = repoVeicolo;
        _repoCliente = repoCliente;
    }

    public VeicoloDto? CercaPerCodice(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return null;

        Veicolo? veicolo = _repoVeicolo.GetByCodice(codice.Trim().ToUpper());
        return veicolo is null ? null : Map(veicolo);
    }

    public int? CercaVeicoloIdPerTarga(string targa)
    {
        if (string.IsNullOrWhiteSpace(targa))
            return null;

        Veicolo? veicolo = _repoVeicolo.GetByTarga(targa.Trim().ToUpper());
        return veicolo?.VeicoloId;
    }

    public IEnumerable<VeicoloDto> CercaTutti()
    {
        return _repoVeicolo.GetAll().Select(Map).ToList();
    }

    public ServiceResult Inserisci(VeicoloDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Targa) ||
            string.IsNullOrWhiteSpace(entity.Marca) ||
            string.IsNullOrWhiteSpace(entity.Modello) ||
            string.IsNullOrWhiteSpace(entity.CodiceCliente))
            return ServiceResult.Fail("Targa, marca, modello e cliente sono obbligatori.");

        Cliente? cliente = _repoCliente.GetByCodice(entity.CodiceCliente.Trim().ToUpper());
        if (cliente is null)
            return ServiceResult.NotFound("Cliente non trovato.");

        string targa = entity.Targa.Trim().ToUpper();

        // se non arriva un codice lo genero automaticamente
        string codice = string.IsNullOrWhiteSpace(entity.Codice)
            ? GeneraCodice()
            : entity.Codice.Trim().ToUpper();

        if (_repoVeicolo.GetByTarga(targa) is not null)
            return ServiceResult.Conflict("Targa già presente in archivio.");

        entity.Codice = codice;

        Veicolo veicolo = new()
        {
            Codice = codice,
            Targa = targa,
            Marca = entity.Marca.Trim(),
            Modello = entity.Modello.Trim(),
            Anno = entity.Anno,
            ClienteRif = cliente.ClienteId
        };

        return _repoVeicolo.Create(veicolo)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante il salvataggio.");
    }

    public ServiceResult Aggiorna(VeicoloDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        Veicolo? veicolo = _repoVeicolo.GetByCodice(entity.Codice.Trim().ToUpper());
        if (veicolo is null)
            return ServiceResult.NotFound("Veicolo non trovato.");

        if (!string.IsNullOrWhiteSpace(entity.Targa))
            veicolo.Targa = entity.Targa.Trim().ToUpper();

        if (!string.IsNullOrWhiteSpace(entity.Marca))
            veicolo.Marca = entity.Marca.Trim();

        if (!string.IsNullOrWhiteSpace(entity.Modello))
            veicolo.Modello = entity.Modello.Trim();

        if (entity.Anno != 0)
            veicolo.Anno = entity.Anno;

        if (!string.IsNullOrWhiteSpace(entity.CodiceCliente))
        {
            Cliente? cliente = _repoCliente.GetByCodice(entity.CodiceCliente.Trim().ToUpper());
            if (cliente is null)
                return ServiceResult.NotFound("Cliente non trovato.");
            veicolo.ClienteRif = cliente.ClienteId;
        }

        return _repoVeicolo.Update(veicolo)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'aggiornamento.");
    }

    public ServiceResult Elimina(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        Veicolo? veicolo = _repoVeicolo.GetByCodice(codice.Trim().ToUpper());
        if (veicolo is null)
            return ServiceResult.NotFound("Veicolo non trovato.");

        // il repository blocca l'eliminazione se ci sono interventi collegati
        return _repoVeicolo.Delete(veicolo.VeicoloId)
            ? ServiceResult.Ok()
            : ServiceResult.Fail("Impossibile eliminare: il veicolo ha interventi collegati.");

    }

    // genera codice progressivo tipo VEI2026-001
    private string GeneraCodice()
    {
        int anno = DateTime.Today.Year;
        string pref = $"VEI{anno}-";
        int max = _repoVeicolo.GetAll()
            .Select(v => v.Codice)
            .Where(c => c.StartsWith(pref))
            .Select(c => int.TryParse(c[pref.Length..], out int n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"{pref}{(max + 1):D3}";
    }

    // mapping entity -- dto
    private static VeicoloDto Map(Veicolo veicolo) => new()
    {
        Codice = veicolo.Codice,
        Targa = veicolo.Targa,
        Marca = veicolo.Marca,
        Modello = veicolo.Modello,
        Anno = veicolo.Anno,
        CodiceCliente = veicolo.Cliente?.Codice ?? string.Empty,
        NomeCliente = veicolo.Cliente?.Nome ?? string.Empty,
        CognomeCliente = veicolo.Cliente?.Cognome ?? string.Empty
    };
}
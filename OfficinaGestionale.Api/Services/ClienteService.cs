using OfficinaGestionale.Api.DTOs;
using OfficinaGestionale.Api.Models;
using OfficinaGestionale.Api.Repositories;

namespace OfficinaGestionale.Api.Services;

public class ClienteService : IService<ClienteDto>
{
    private readonly IRepoLettura<Cliente> _repoLettura;
    private readonly IRepoScrittura<Cliente> _repoScrittura;
    private readonly IVeicoloRepo _repoVeicoli;

    public ClienteService(
        IRepoLettura<Cliente> repoLettura,
        IRepoScrittura<Cliente> repoScrittura,
        IVeicoloRepo repoVeicoli)
    {
        _repoLettura = repoLettura;
        _repoScrittura = repoScrittura;
        _repoVeicoli = repoVeicoli;
    }

    public ClienteDto? CercaPerCodice(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return null;

        Cliente? cliente = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        return cliente is null ? null : Map(cliente);
    }

    public ClienteDto? CercaPerId(int id)
    {
        Cliente? cliente = _repoLettura.GetById(id);
        return cliente is null ? null : Map(cliente);
    }

    public int? CercaClienteIdPerCodice(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return null;

        Cliente? cliente = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        return cliente?.ClienteId;
    }

    public IEnumerable<ClienteDto> CercaTutti()
    {
        return _repoLettura.GetAll().Select(Map).ToList();
    }

    public ServiceResult Inserisci(ClienteDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Nome) || string.IsNullOrWhiteSpace(entity.Cognome))
            return ServiceResult.Fail("Nome e cognome sono obbligatori.");

        // se non arriva un codice lo genero automaticamente
        string codice = string.IsNullOrWhiteSpace(entity.Codice)
            ? GeneraCodice()
            : entity.Codice.Trim().ToUpper();

        if (_repoLettura.GetByCodice(codice) is not null)
            return ServiceResult.Conflict("Codice già in uso.");

        entity.Codice = codice;

        Cliente cliente = new()
        {
            Codice = codice,
            Nome = entity.Nome.Trim(),
            Cognome = entity.Cognome.Trim(),
            Indirizzo = entity.Indirizzo?.Trim(),
            Telefono = entity.Telefono?.Trim(),
            Email = entity.Email?.Trim(),
            CodiceFiscale = entity.CodiceFiscale?.Trim().ToUpper(),
            PartitaIva = entity.PartitaIva?.Trim()
        };

        return _repoScrittura.Create(cliente)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante il salvataggio.");
    }

    public ServiceResult Aggiorna(ClienteDto entity)
    {
        if (string.IsNullOrWhiteSpace(entity.Codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        Cliente? cliente = _repoLettura.GetByCodice(entity.Codice.Trim().ToUpper());
        if (cliente is null)
            return ServiceResult.NotFound("Cliente non trovato.");

        if (!string.IsNullOrWhiteSpace(entity.Nome))
            cliente.Nome = entity.Nome.Trim();

        if (!string.IsNullOrWhiteSpace(entity.Cognome))
            cliente.Cognome = entity.Cognome.Trim();

        cliente.Indirizzo = entity.Indirizzo?.Trim();
        cliente.Telefono = entity.Telefono?.Trim();
        cliente.Email = entity.Email?.Trim();
        cliente.CodiceFiscale = entity.CodiceFiscale?.Trim().ToUpper();
        cliente.PartitaIva = entity.PartitaIva?.Trim();

        return _repoScrittura.Update(cliente)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'aggiornamento.");
    }

    public ServiceResult Elimina(string codice)
    {
        if (string.IsNullOrWhiteSpace(codice))
            return ServiceResult.Fail("Codice obbligatorio.");

        Cliente? cliente = _repoLettura.GetByCodice(codice.Trim().ToUpper());
        if (cliente is null)
            return ServiceResult.NotFound("Cliente non trovato.");

        // non elimino clienti con veicoli collegati
        if (_repoVeicoli.GetAll().Any(v => v.ClienteRif == cliente.ClienteId))
            return ServiceResult.Fail("Impossibile eliminare: il cliente ha veicoli collegati.");

        return _repoScrittura.Delete(cliente.ClienteId)
            ? ServiceResult.Ok()
            : ServiceResult.ServerError("Errore durante l'eliminazione.");
    }

    // genera codice progressivo tipo CLI2026-001
    private string GeneraCodice()
    {
        int anno = DateTime.Today.Year;
        string pref = $"CLI{anno}-";
        int max = _repoLettura.GetAll()
            .Select(c => c.Codice)
            .Where(c => c.StartsWith(pref))
            .Select(c => int.TryParse(c[pref.Length..], out int n) ? n : 0)
            .DefaultIfEmpty(0)
            .Max();
        return $"{pref}{(max + 1):D3}";
    }

    // mapping entity -> dto
    private static ClienteDto Map(Cliente cliente) => new()
    {
        Codice = cliente.Codice,
        Nome = cliente.Nome,
        Cognome = cliente.Cognome,
        Indirizzo = cliente.Indirizzo,
        Telefono = cliente.Telefono,
        Email = cliente.Email,
        CodiceFiscale = cliente.CodiceFiscale,
        PartitaIva = cliente.PartitaIva
    };
}
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficinaGestionale.Api.Context;
using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.Repositories;

public class ClienteRepo : IRepoLettura<Cliente>, IRepoScrittura<Cliente>
{
    private readonly OfficinaContext _context;
    private readonly ILogger<ClienteRepo> _logger;

    public ClienteRepo(OfficinaContext context, ILogger<ClienteRepo> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IEnumerable<Cliente> GetAll()
    {
        return _context.Clienti
            .Include(c => c.Veicoli)
            .OrderBy(c => c.Cognome)
            .ThenBy(c => c.Nome)
            .ToList();
    }

    public Cliente? GetById(int id)
    {
        return _context.Clienti
            .Include(c => c.Veicoli)
            .FirstOrDefault(c => c.ClienteId == id);
    }

    public Cliente? GetByCodice(string codice)
    {
        return _context.Clienti
            .Include(c => c.Veicoli)
            .FirstOrDefault(c => c.Codice == codice);
    }

    public bool Create(Cliente entity)
    {
        try
        {
            _context.Clienti.Add(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la creazione del cliente {Codice}.", entity.Codice);
            return false;
        }
    }

    public bool Update(Cliente entity)
    {
        try
        {
            _context.Clienti.Update(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'aggiornamento del cliente {Codice}.", entity.Codice);
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            Cliente? entity = _context.Clienti.FirstOrDefault(c => c.ClienteId == id);
            if (entity is null)
                return false;

            // evito di eliminare clienti con veicoli collegati
            bool hasVeicoli = _context.Veicoli.Any(v => v.ClienteRif == id);
            if (hasVeicoli)
                return false;

            _context.Clienti.Remove(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione del cliente con ID {Id}.", id);
            return false;
        }
    }
}

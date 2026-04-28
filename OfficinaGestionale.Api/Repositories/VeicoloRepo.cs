using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficinaGestionale.Api.Context;
using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.Repositories;

public class VeicoloRepo : IVeicoloRepo
{
    private readonly OfficinaContext _context;
    private readonly ILogger<VeicoloRepo> _logger;

    public VeicoloRepo(OfficinaContext context, ILogger<VeicoloRepo> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IEnumerable<Veicolo> GetAll()
    {
        return _context.Veicoli
            .Include(v => v.Cliente)
            .OrderBy(v => v.Targa)
            .ToList();
    }

    public Veicolo? GetById(int id)
    {
        return _context.Veicoli
            .Include(v => v.Cliente)
            .FirstOrDefault(v => v.VeicoloId == id);
    }

    public Veicolo? GetByCodice(string codice)
    {
        return _context.Veicoli
            .Include(v => v.Cliente)
            .FirstOrDefault(v => v.Codice == codice);
    }

    public Veicolo? GetByTarga(string targa)
    {
        return _context.Veicoli
            .Include(v => v.Cliente)
            .FirstOrDefault(v => v.Targa == targa);
    }
    public IEnumerable<Veicolo> GetByClienteId(int clienteId)
    {
        return _context.Veicoli
            .Include(v => v.Cliente)
            .Where(v => v.ClienteRif == clienteId)
            .OrderBy(v => v.Targa)
            .ToList();
    }

    public bool Create(Veicolo entity)
    {
        try
        {
            _context.Veicoli.Add(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la creazione del veicolo {Targa}.", entity.Targa);
            return false;
        }
    }

    public bool Update(Veicolo entity)
    {
        try
        {
            _context.Veicoli.Update(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'aggiornamento del veicolo {Targa}.", entity.Targa);
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            Veicolo? entity = _context.Veicoli.FirstOrDefault(v => v.VeicoloId == id);
            if (entity is null)
                return false;

            var hasInterventi = _context.Interventi.Any(i => i.VeicoloRif == id);
            if (hasInterventi) return false;

            _context.Veicoli.Remove(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione del veicolo con ID {Id}.", id);
            return false;
        }
    }
}

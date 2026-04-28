using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficinaGestionale.Api.Context;
using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.Repositories;

public class InterventoRepo : IRepoLettura<Intervento>, IRepoScrittura<Intervento>
{
    private readonly OfficinaContext _context;
    private readonly ILogger<InterventoRepo> _logger;

    public InterventoRepo(OfficinaContext context, ILogger<InterventoRepo> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IEnumerable<Intervento> GetAll()
    {
        // carico anche veicolo e cliente per avere i dati completi
        return _context.Interventi
            .Include(i => i.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .OrderByDescending(i => i.DataIngresso)
            .ToList();
    }

    public Intervento? GetById(int id)
    {
        return _context.Interventi
            .Include(i => i.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .FirstOrDefault(i => i.InterventoId == id);
    }

    public Intervento? GetByCodice(string codice)
    {
        return _context.Interventi
            .Include(i => i.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .FirstOrDefault(i => i.Codice == codice);
    }

    public bool Create(Intervento entity)
    {
        try
        {
            _context.Interventi.Add(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la creazione dell'intervento {Codice}.", entity.Codice);
            return false;
        }
    }

    public bool Update(Intervento entity)
    {
        try
        {
            _context.Interventi.Update(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'aggiornamento dell'intervento {Codice}.", entity.Codice);
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            Intervento? entity = _context.Interventi.FirstOrDefault(i => i.InterventoId == id);
            if (entity is null)
                return false;

            _context.Interventi.Remove(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione dell'intervento con ID {Id}.", id);
            return false;
        }
    }
}

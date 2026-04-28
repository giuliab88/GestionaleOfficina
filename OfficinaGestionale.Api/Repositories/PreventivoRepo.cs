using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficinaGestionale.Api.Context;
using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.Repositories;

public class PreventivoRepo : IRepoLettura<Preventivo>, IRepoScrittura<Preventivo>
{
    private readonly OfficinaContext _context;
    private readonly ILogger<PreventivoRepo> _logger;

    public PreventivoRepo(OfficinaContext context, ILogger<PreventivoRepo> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IEnumerable<Preventivo> GetAll()
    {
        return _context.Preventivi
            .Include(p => p.Voci)
            .Include(p => p.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .OrderByDescending(p => p.Data)
            .ToList();
    }

    public Preventivo? GetById(int id)
    {
        return _context.Preventivi
            .Include(p => p.Voci)
            .Include(p => p.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .FirstOrDefault(p => p.PreventivoId == id);
    }

    public Preventivo? GetByCodice(string codice)
    {
        return _context.Preventivi
            .Include(p => p.Voci)
            .Include(p => p.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .FirstOrDefault(p => p.Codice == codice);
    }

    public bool Create(Preventivo entity)
    {
        try
        {
            _context.Preventivi.Add(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la creazione del preventivo {Codice}.", entity.Codice);
            return false;
        }
    }

    public bool Update(Preventivo entity)
    {
        try
        {
            Preventivo? esistente = _context.Preventivi
                .Include(p => p.Voci)
                .FirstOrDefault(p => p.PreventivoId == entity.PreventivoId);

            if (esistente is null)
                return false;

            esistente.Data        = entity.Data;
            esistente.ValidoFino  = entity.ValidoFino;
            esistente.Note        = entity.Note;
            esistente.Stato       = entity.Stato;
            esistente.AliquotaIva = entity.AliquotaIva;
            esistente.VeicoloRif  = entity.VeicoloRif;

            // rimpiazza le voci
            _context.VociPreventivo.RemoveRange(esistente.Voci);
            esistente.Voci.Clear();
            foreach (var voce in entity.Voci)
                esistente.Voci.Add(new VocePreventivo
                {
                    Descrizione    = voce.Descrizione,
                    Quantita       = voce.Quantita,
                    PrezzoUnitario = voce.PrezzoUnitario
                });

            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'aggiornamento del preventivo {Codice}.", entity.Codice);
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            Preventivo? entity = _context.Preventivi
                .Include(p => p.Voci)
                .FirstOrDefault(p => p.PreventivoId == id);

            if (entity is null)
                return false;

            _context.Preventivi.Remove(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione del preventivo con ID {Id}.", id);
            return false;
        }
    }
}

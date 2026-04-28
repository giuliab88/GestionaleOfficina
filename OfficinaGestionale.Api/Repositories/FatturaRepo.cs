using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OfficinaGestionale.Api.Context;
using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.Repositories;

public class FatturaRepo : IRepoLettura<Fattura>, IRepoScrittura<Fattura>
{
    private readonly OfficinaContext _context;
    private readonly ILogger<FatturaRepo> _logger;

    public FatturaRepo(OfficinaContext context, ILogger<FatturaRepo> logger)
    {
        _context = context;
        _logger = logger;
    }

    public IEnumerable<Fattura> GetAll()
    {
        // carico anche righe, veicolo e cliente per mostrare la fattura completa
        return _context.Fatture
            .Include(f => f.Righe)
            .Include(f => f.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .Include(f => f.Preventivo)
            .OrderByDescending(f => f.DataEmissione)
            .ToList();
    }

    public Fattura? GetById(int id)
    {
        return _context.Fatture
            .Include(f => f.Righe)
            .Include(f => f.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .Include(f => f.Preventivo)
            .FirstOrDefault(f => f.FatturaId == id);
    }

    public Fattura? GetByCodice(string codice)
    {
        return _context.Fatture
            .Include(f => f.Righe)
            .Include(f => f.Veicolo)
                .ThenInclude(v => v!.Cliente)
            .Include(f => f.Preventivo)
            .FirstOrDefault(f => f.Codice == codice);
    }

    public bool Create(Fattura entity)
    {
        try
        {
            _context.Fatture.Add(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante la creazione della fattura {Codice}.", entity.Codice);
            return false;
        }
    }

    public bool Update(Fattura entity)
    {
        try
        {
            Fattura? esistente = _context.Fatture
                .Include(f => f.Righe)
                .FirstOrDefault(f => f.FatturaId == entity.FatturaId);

            if (esistente is null)
                return false;

            esistente.DataEmissione     = entity.DataEmissione;
            esistente.DataScadenza      = entity.DataScadenza;
            esistente.Note              = entity.Note;
            esistente.Stato             = entity.Stato;
            esistente.AliquotaIva       = entity.AliquotaIva;
            esistente.ModalitaPagamento = entity.ModalitaPagamento;
            esistente.VeicoloRif        = entity.VeicoloRif;
            esistente.PreventivoRif     = entity.PreventivoRif;

            // aggiorno le righe ricreandole, così evito differenze tra vecchie e nuove
            _context.RigheFattura.RemoveRange(esistente.Righe);
            esistente.Righe.Clear();
            foreach (var riga in entity.Righe)
                esistente.Righe.Add(new RigaFattura
                {
                    Descrizione    = riga.Descrizione,
                    Quantita       = riga.Quantita,
                    PrezzoUnitario = riga.PrezzoUnitario
                });

            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'aggiornamento della fattura {Codice}.", entity.Codice);
            return false;
        }
    }

    public bool Delete(int id)
    {
        try
        {
            Fattura? entity = _context.Fatture
                .Include(f => f.Righe)
                .FirstOrDefault(f => f.FatturaId == id);

            if (entity is null)
                return false;

            _context.Fatture.Remove(entity);
            _context.SaveChanges();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Errore durante l'eliminazione della fattura con ID {Id}.", id);
            return false;
        }
    }
}

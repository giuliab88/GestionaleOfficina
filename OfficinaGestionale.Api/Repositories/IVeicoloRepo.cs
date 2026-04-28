using OfficinaGestionale.Api.Models;

namespace OfficinaGestionale.Api.Repositories;

public interface IVeicoloRepo : IRepoLettura<Veicolo>, IRepoScrittura<Veicolo>
{
    Veicolo? GetByTarga(string targa);
}
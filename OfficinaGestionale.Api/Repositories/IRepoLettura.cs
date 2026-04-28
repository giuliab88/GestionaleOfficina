namespace OfficinaGestionale.Api.Repositories;

public interface IRepoLettura<T>
{
    IEnumerable<T> GetAll();
    T? GetById(int id);
    T? GetByCodice(string codice);
}

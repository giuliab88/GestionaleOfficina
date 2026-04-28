namespace OfficinaGestionale.Api.Services;

public interface IService<T>
{
    T? CercaPerCodice(string codice);
    IEnumerable<T> CercaTutti();
    ServiceResult Inserisci(T entity);
    ServiceResult Aggiorna(T entity);
    ServiceResult Elimina(string codice);
}

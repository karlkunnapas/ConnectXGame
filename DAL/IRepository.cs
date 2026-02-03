namespace DAL;

public interface IRepository<TData>
{
    Dictionary<int, (Guid, string)> List();
    Task<Dictionary<int, (Guid, string)>> ListAsync();

    // crud
    string Save(TData data);
    TData Load(Guid id);
    void Delete(Guid id);
}
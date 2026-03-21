namespace DAL.DataAccessLayer.Repositories;

public interface IRepository<T> where T : class
{
    IEnumerable<T> GetAll();
    T? GetById(Guid id);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);
    void DeleteById(Guid id);
    IEnumerable<T> Find(Func<T, bool> predicate);
}

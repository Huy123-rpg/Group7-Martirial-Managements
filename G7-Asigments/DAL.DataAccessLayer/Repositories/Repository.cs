using DAL.DataAccessLayer.Context;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataAccessLayer.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    protected readonly WarehouseDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(WarehouseDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public IEnumerable<T> GetAll() => _dbSet.ToList();

    public T? GetById(Guid id) => _dbSet.Find(id);

    public void Add(T entity) => _dbSet.Add(entity);

    public void Update(T entity) => _dbSet.Update(entity);

    public void Delete(T entity) => _dbSet.Remove(entity);

    public void DeleteById(Guid id)
    {
        var entity = GetById(id);
        if (entity != null) Delete(entity);
    }

    public IEnumerable<T> Find(Func<T, bool> predicate) => _dbSet.Where(predicate).ToList();
}

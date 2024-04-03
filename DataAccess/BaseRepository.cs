using eLearningApi.DataAccess.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace eLearningApi.DataAccess;

public class BaseRepository<T> : IRepository<T> where T : class
{
    private readonly AuditService _dateService;
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public BaseRepository(AppDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
        _dateService = new AuditService();
    }

    public EntityEntry<T> Delete(T entity)
    {
        if (_context.Entry(entity).State == EntityState.Detached)
        {
            _dbSet.Attach(entity);
        }

        return _dbSet.Remove(entity);
    }

    public EntityEntry<T> Delete(params object[] id)
    {
        T? entity = Find(id);

        if (entity is null)
        {
            return default!;
        }

        return Delete(entity);
    }

    public T? Find(params object[] keys)
    {
        return _dbSet.Find(keys);
    }

    public IEnumerable<T> Get(Expression<Func<T, bool>> expression = default!, params string[] include)
    {
        var query = _dbSet.AsQueryable();

        if (include.Any())
        {
            foreach (var propertyName in include)
            {
                query = query.Include(propertyName);
            }
        }

        if (expression != null)
        {
            query = query.Where(expression);
        }

        return query.ToList();
    }

    public EntityEntry<T> Insert(T entity)
    {
        _dateService.OnCreate(entity);

        return _dbSet.Add(entity);
    }

    public EntityEntry<T> Update(T entity)
    {
        _dateService.OnUpdate(entity);

        return _dbSet.Update(entity);
    }
}

using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Linq.Expressions;

namespace eLearningApi.DataAccess.Interfaces;

public interface IRepository<T> where T : class
{
    IEnumerable<T> Get(Expression<Func<T, bool>> expression = default!, params string[] include);
    T Find(params object[] keys);
    EntityEntry<T> Insert(T entity);
    EntityEntry<T> Update(T entity);
    EntityEntry<T> Delete(T entity);
    EntityEntry<T> Delete(params object[] id);
}

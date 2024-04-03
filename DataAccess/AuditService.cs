using eLearningApi.Models.Interfaces;

namespace eLearningApi.DataAccess;

public class AuditService
{
    public void OnCreate<T>(T entity)
    {
        if (entity is IAuditableEntity auditableEntity)
        {
            auditableEntity.CreatedAt = DateTime.Now;
            OnUpdate(auditableEntity);
        }
    }

    public void OnUpdate<T>(T entity)
    {
        if (entity is IAuditableEntity auditableEntity)
        {
            auditableEntity.UpdatedAt = DateTime.Now;
        }
    }
}

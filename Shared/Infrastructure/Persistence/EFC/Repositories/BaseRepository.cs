using System.Linq.Expressions;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;

namespace CeTe.Shared.Infrastructure.Persistence.EFC.Repositories;

public abstract class BaseRepository(CeTeDb db)
{
    protected CeTeDb Database => db;
    protected Task<List<T>> List<T>(Expression<Func<T, bool>>? predicate = null) where T : Entity =>
        predicate == null ? db.Set<T>().ToListAsync() : db.Set<T>().Where(predicate).ToListAsync();

    protected Task<T?> Find<T>(Guid id) where T : Entity =>
        db.Set<T>().SingleOrDefaultAsync(entity => entity.Id == id);

    protected void Add<T>(T entity) where T : Entity
    {
        if (entity is TenantEntity tenantEntity && tenantEntity.TenantId == Guid.Empty)
            tenantEntity.TenantId = db.CurrentTenantId;
        db.Set<T>().Add(entity);
    }
}

using LUMS.Core.Repositories;
using LUMS.Infrastructure.Data;

namespace LUMS.Infrastructure.Repositories;

public abstract class EfRepository<T> : IRepository<T> where T : class
{
    protected EfRepository(LumsDbContext db)
    {
        Db = db;
    }

    protected LumsDbContext Db { get; }

    public virtual async Task<T?> GetByIdAsync(object id, CancellationToken ct = default) =>
        await Db.Set<T>().FindAsync(new[] { id }, ct);

    public virtual async Task AddAsync(T entity, CancellationToken ct = default) =>
        await Db.Set<T>().AddAsync(entity, ct);

    public virtual void Update(T entity) => Db.Set<T>().Update(entity);

    public virtual void Remove(T entity) => Db.Set<T>().Remove(entity);
}

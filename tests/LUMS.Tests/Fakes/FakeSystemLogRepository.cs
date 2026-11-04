using LUMS.Core.Entities;
using LUMS.Core.Interfaces.Repositories;

namespace LUMS.Tests.Fakes;

public sealed class FakeSystemLogRepository : ISystemLogRepository
{
    public List<SystemLog> Items { get; } = new();

    public Task<IReadOnlyList<SystemLog>> GetRecentAsync(int take, CancellationToken ct = default) =>
        Task.FromResult<IReadOnlyList<SystemLog>>(Items.OrderByDescending(l => l.Timestamp).Take(take).ToList());

    public Task<SystemLog?> GetByIdAsync(object id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(l => l.Id.Equals(id)));

    public Task AddAsync(SystemLog entity, CancellationToken ct = default)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(SystemLog entity)
    {
    }

    public void Remove(SystemLog entity) => Items.Remove(entity);
}

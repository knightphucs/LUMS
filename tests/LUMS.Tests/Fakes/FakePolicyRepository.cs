using LUMS.Core.Entities;
using LUMS.Core.Interfaces.Repositories;

namespace LUMS.Tests.Fakes;

public sealed class FakePolicyRepository : IPolicyRepository
{
    public List<PolicyConfiguration> Items { get; } = new();

    public Task<PolicyConfiguration?> GetCurrentAsync(CancellationToken ct = default) =>
        Task.FromResult(Items.LastOrDefault());

    public Task<PolicyConfiguration?> GetByIdAsync(object id, CancellationToken ct = default) =>
        Task.FromResult(Items.FirstOrDefault(p => p.Id.Equals(id)));

    public Task AddAsync(PolicyConfiguration entity, CancellationToken ct = default)
    {
        Items.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(PolicyConfiguration entity)
    {
    }

    public void Remove(PolicyConfiguration entity) => Items.Remove(entity);
}

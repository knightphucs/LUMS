using LUMS.Core.Dtos;

namespace LUMS.Core.Services.Interfaces;

public interface IPolicyService
{
    Task<PolicyDto> GetCurrentAsync(CancellationToken ct = default);
    Task UpdateAsync(UpdatePolicyRequest request, int adminId, CancellationToken ct = default);
}

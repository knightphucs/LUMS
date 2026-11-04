using LUMS.Core.Dtos;

namespace LUMS.Core.Interfaces.Services;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string emailOrCode, string password, CancellationToken ct = default);
}

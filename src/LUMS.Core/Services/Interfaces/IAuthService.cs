using LUMS.Core.Dtos;

namespace LUMS.Core.Services.Interfaces;

public interface IAuthService
{
    Task<LoginResult> LoginAsync(string emailOrCode, string password, CancellationToken ct = default);
}

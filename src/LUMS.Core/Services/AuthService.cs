using LUMS.Core.Dtos;
using LUMS.Core.Enums;
using LUMS.Core.Interfaces.Common;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Core.Interfaces.Services;

namespace LUMS.Core.Services;

public class AuthService : IAuthService
{
    private readonly IAccountRepository _accounts;
    private readonly IPasswordHasher _hasher;

    public AuthService(IAccountRepository accounts, IPasswordHasher hasher)
    {
        _accounts = accounts;
        _hasher = hasher;
    }

    public async Task<LoginResult> LoginAsync(string emailOrCode, string password, CancellationToken ct = default)
    {
        var account = string.IsNullOrWhiteSpace(emailOrCode)
            ? null
            : await _accounts.GetByLoginAsync(emailOrCode.Trim(), ct);

        if (account is null || !_hasher.Verify(account.PasswordHash, password))
        {
            return LoginResult.Fail("Email/MSSV hoặc mật khẩu không đúng.");
        }

        return account.Status switch
        {
            AccountStatus.ChoXacNhan => LoginResult.Fail("Tài khoản đang chờ Admin xác nhận."),
            AccountStatus.Khoa => LoginResult.Fail("Tài khoản đã bị khóa. Vui lòng liên hệ thư viện."),
            _ => LoginResult.Success(new AuthenticatedUser(account.Id, account.FullName, account.Code, account.Role))
        };
    }
}

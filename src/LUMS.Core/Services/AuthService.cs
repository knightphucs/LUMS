using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Enums;
using LUMS.Core.Interfaces.Common;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Core.Interfaces.Services;

namespace LUMS.Core.Services;

public class AuthService : IAuthService
{
    private const string InvalidCredentials = "Email/MSSV hoặc mật khẩu không đúng.";

    private readonly IAccountRepository _accounts;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public AuthService(IAccountRepository accounts, IPasswordHasher hasher, IUnitOfWork uow, IClock clock)
    {
        _accounts = accounts;
        _hasher = hasher;
        _uow = uow;
        _clock = clock;
    }

    public async Task<LoginResult> LoginAsync(string emailOrCode, string password, CancellationToken ct = default)
    {
        var account = string.IsNullOrWhiteSpace(emailOrCode)
            ? null
            : await _accounts.GetByLoginAsync(emailOrCode.Trim(), ct);

        if (account is null)
        {
            return LoginResult.Fail(InvalidCredentials);
        }

        var now = _clock.Now;

        // Đang trong 15 phút tạm khóa: từ chối, kể cả khi mật khẩu đúng.
        if (account.LockoutEnd is { } lockoutEnd && lockoutEnd > now)
        {
            return LoginResult.LockedOut(MinutesUntil(lockoutEnd, now));
        }

        if (!_hasher.Verify(account.PasswordHash, password))
        {
            return await RegisterFailedAttemptAsync(account, now, ct);
        }

        // Đúng mật khẩu → xóa bộ đếm và mốc khóa đã hết hạn.
        if (account.FailedLoginCount != 0 || account.LockoutEnd is not null)
        {
            account.FailedLoginCount = 0;
            account.LockoutEnd = null;
            await SaveAsync(account, ct);
        }

        return account.Status switch
        {
            AccountStatus.ChoXacNhan => LoginResult.Fail("Tài khoản đang chờ Admin xác nhận."),
            AccountStatus.Khoa => LoginResult.Fail("Tài khoản đã bị khóa. Vui lòng liên hệ thư viện."),
            _ => LoginResult.Success(new AuthenticatedUser(account.Id, account.FullName, account.Code, account.Role))
        };
    }

    private async Task<LoginResult> RegisterFailedAttemptAsync(Account account, DateTime now, CancellationToken ct)
    {
        // Lần khóa trước đã hết hạn → bắt đầu đếm lại từ đầu.
        if (account.LockoutEnd is not null)
        {
            account.LockoutEnd = null;
            account.FailedLoginCount = 0;
        }

        account.FailedLoginCount++;

        if (account.FailedLoginCount >= AuthPolicy.MaxFailedAttempts)
        {
            account.FailedLoginCount = 0;
            account.LockoutEnd = now + AuthPolicy.LockoutDuration;
            await SaveAsync(account, ct);
            return LoginResult.LockedOut(MinutesUntil(account.LockoutEnd.Value, now));
        }

        await SaveAsync(account, ct);
        return LoginResult.Fail(InvalidCredentials);
    }

    private async Task SaveAsync(Account account, CancellationToken ct)
    {
        _accounts.Update(account);
        await _uow.SaveChangesAsync(ct);
    }

    private static int MinutesUntil(DateTime end, DateTime now) =>
        Math.Max(1, (int)Math.Ceiling((end - now).TotalMinutes));
}

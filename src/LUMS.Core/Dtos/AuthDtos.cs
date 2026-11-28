using LUMS.Core.Enums;

namespace LUMS.Core.Dtos;

public record AuthenticatedUser(int Id, string FullName, string Code, Role Role);

/// <param name="LockoutMinutesRemaining">Số phút còn lại của lần tạm khóa (làm tròn lên); null nếu không bị tạm khóa.</param>
public record LoginResult(bool Succeeded, string? Error, AuthenticatedUser? User, int? LockoutMinutesRemaining = null)
{
    public bool IsLockedOut => LockoutMinutesRemaining is not null;

    public static LoginResult Success(AuthenticatedUser user) => new(true, null, user);

    public static LoginResult Fail(string error) => new(false, error, null);

    public static LoginResult LockedOut(int minutesRemaining) => new(
        false,
        $"Bạn đã nhập sai mật khẩu quá {AuthPolicy.MaxFailedAttempts} lần. Đăng nhập tạm khóa, vui lòng thử lại sau {minutesRemaining} phút.",
        null,
        minutesRemaining);
}

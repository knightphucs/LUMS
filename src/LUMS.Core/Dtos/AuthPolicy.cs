namespace LUMS.Core.Dtos;

/// <summary>Quy định khóa đăng nhập (SRS 3.1, FR-00-01).</summary>
public static class AuthPolicy
{
    /// <summary>Sai mật khẩu liên tiếp bao nhiêu lần thì tạm khóa.</summary>
    public const int MaxFailedAttempts = 5;

    /// <summary>Thời gian tạm khóa.</summary>
    public static readonly TimeSpan LockoutDuration = TimeSpan.FromMinutes(15);
}

using LUMS.Core.Entities;
using LUMS.Core.Interfaces.Common;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Core.Interfaces.Services;

namespace LUMS.Core.Services;

public class AccessLogService : IAccessLogService
{
    public const string ActionPrefix = "Truy cập trái phép: ";
    public const string RelatedObjectName = "RBAC";

    // Khớp độ dài cột HanhDong (NHAT_KY_HE_THONG) trong SystemLogConfiguration.
    private const int MaxActionLength = 255;

    private readonly ISystemLogRepository _logs;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public AccessLogService(ISystemLogRepository logs, IUnitOfWork uow, IClock clock)
    {
        _logs = logs;
        _uow = uow;
        _clock = clock;
    }

    public async Task LogUnauthorizedAccessAsync(int accountId, string path, CancellationToken ct = default)
    {
        var action = ActionPrefix + (string.IsNullOrWhiteSpace(path) ? "/" : path.Trim());
        if (action.Length > MaxActionLength)
        {
            action = action[..(MaxActionLength - 1)] + "…";
        }

        await _logs.AddAsync(new SystemLog
        {
            AccountId = accountId,
            Action = action,
            RelatedObject = RelatedObjectName,
            Timestamp = _clock.Now
        }, ct);

        await _uow.SaveChangesAsync(ct);
    }
}

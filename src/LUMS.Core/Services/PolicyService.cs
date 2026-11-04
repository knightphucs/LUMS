using LUMS.Core.Dtos;
using LUMS.Core.Entities;
using LUMS.Core.Exceptions;
using LUMS.Core.Interfaces.Common;
using LUMS.Core.Interfaces.Repositories;
using LUMS.Core.Interfaces.Services;

namespace LUMS.Core.Services;

public class PolicyService : IPolicyService
{
    private readonly IPolicyRepository _policies;
    private readonly ISystemLogRepository _logs;
    private readonly IUnitOfWork _uow;
    private readonly IClock _clock;

    public PolicyService(IPolicyRepository policies, ISystemLogRepository logs, IUnitOfWork uow, IClock clock)
    {
        _policies = policies;
        _logs = logs;
        _uow = uow;
        _clock = clock;
    }

    public async Task<PolicyDto> GetCurrentAsync(CancellationToken ct = default)
    {
        var p = await _policies.GetCurrentAsync(ct) ?? PolicyConfiguration.Default;
        return new PolicyDto(p.MaxLoanDays, p.MaxBooksPerReader, p.MaxReservations, p.FinePerDay);
    }

    public async Task UpdateAsync(UpdatePolicyRequest request, int adminId, CancellationToken ct = default)
    {
        if (request.MaxLoanDays < 1 || request.MaxBooksPerReader < 1 || request.MaxReservations < 0 || request.FinePerDay < 0)
        {
            throw new DomainException("Giá trị cấu hình không hợp lệ: số ngày/số sách phải ≥ 1 và mức phạt không được âm.");
        }

        var now = _clock.Now;

        await _policies.AddAsync(new PolicyConfiguration
        {
            MaxLoanDays = request.MaxLoanDays,
            MaxBooksPerReader = request.MaxBooksPerReader,
            MaxReservations = request.MaxReservations,
            FinePerDay = request.FinePerDay,
            UpdatedById = adminId,
            EffectiveFrom = now
        }, ct);

        await _logs.AddAsync(new SystemLog
        {
            AccountId = adminId,
            Action = $"Cập nhật cấu hình: Số ngày mượn tối đa = {request.MaxLoanDays}; Số sách mượn cùng lúc = {request.MaxBooksPerReader}",
            RelatedObject = "PolicyConfiguration",
            Timestamp = now
        }, ct);

        await _uow.SaveChangesAsync(ct);
    }
}

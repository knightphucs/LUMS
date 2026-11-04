using LUMS.Core.Enums;

namespace LUMS.Core.Dtos;

public record AuthenticatedUser(int Id, string FullName, string Code, Role Role);

public record LoginResult(bool Succeeded, string? Error, AuthenticatedUser? User)
{
    public static LoginResult Success(AuthenticatedUser user) => new(true, null, user);

    public static LoginResult Fail(string error) => new(false, error, null);
}

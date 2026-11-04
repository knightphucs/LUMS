using LUMS.Core.Abstractions;
using LUMS.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace LUMS.Infrastructure.Services;

public class IdentityPasswordHasher : IPasswordHasher
{
    private readonly PasswordHasher<Account> _inner = new();

    public string Hash(string password) => _inner.HashPassword(null!, password);

    public bool Verify(string passwordHash, string password) =>
        !string.IsNullOrEmpty(passwordHash)
        && _inner.VerifyHashedPassword(null!, passwordHash, password) != PasswordVerificationResult.Failed;
}

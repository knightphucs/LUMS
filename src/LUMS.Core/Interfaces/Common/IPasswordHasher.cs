namespace LUMS.Core.Interfaces.Common;

public interface IPasswordHasher
{
    string Hash(string password);
    bool Verify(string passwordHash, string password);
}

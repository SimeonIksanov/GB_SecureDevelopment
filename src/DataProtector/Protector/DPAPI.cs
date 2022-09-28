using System.Security.Cryptography;

namespace DataProtector.Protector;

public static class DPAPI
{
    public static byte[] OptionalEntropy = new byte[] { 23, 17, 91, 51, 53, 171, 91, 161, 231, 117, 91, 67, 213, 217, 113, 97 };
    public static DataProtectionScope Scope = DataProtectionScope.CurrentUser;

    public static byte[] Protect(this byte[] data) => ProtectedData.Protect(data, OptionalEntropy, Scope);

    public static byte[] Unprotect(this byte[] data) => ProtectedData.Unprotect(data, OptionalEntropy, Scope);
}

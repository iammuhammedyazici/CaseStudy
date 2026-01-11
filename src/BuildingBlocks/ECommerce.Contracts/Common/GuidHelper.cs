using System.Security.Cryptography;
using System.Text;

namespace ECommerce.Contracts.Common;

public static class GuidHelper
{
    public static Guid ToGuid(int id, string prefix = "ID")
    {
        var input = $"{prefix}:{id}";
        var hash = MD5.HashData(Encoding.UTF8.GetBytes(input));
        return new Guid(hash);
    }

    public static int FromGuid(Guid guid, string prefix = "ID")
    {
        throw new NotSupportedException("Reverse GUID to int conversion requires a mapping table");
    }
}

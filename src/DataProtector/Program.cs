using DataProtector.Protector;
using System.Runtime.CompilerServices;
using System.Text;

namespace DataProtector;

internal class Program
{
    static void Main(string[] args)
    {
        //EncodeStringSample();
        EncodeConnectionString();
    }

    private static void EncodeConnectionString()
    {
        var list = new List<ConnectionString>()
        {
            new ConnectionString(){DatabaseName = "db01",Host = "host01",UserName = "user01",Password = "Password01"},
            new ConnectionString(){DatabaseName = "db02",Host = "host02",UserName = "user02",Password = "Password02"},
        };

        var cache = new CacheProvider();
        cache.CacheConnections(list);

        Console.ReadKey(true);

        var list2 = cache.GetConnectionsFromCache();
        foreach (var item in list2)
        {
            Console.WriteLine(item);
        }
    }

    private static void EncodeStringSample()
    {
        string data = "Hello, World!";
        Console.WriteLine("Original: {0}", data);

        string protectedData = data
            .GetBytes()
            .Protect()
            .ToBase64String();
        Console.WriteLine("ProtectedData base64: {0}", protectedData);

        string origin = protectedData
            .FromBase64String()
            .Unprotect()
            .GetString();
        Console.WriteLine("back original: {0}", origin);
    }
}

internal static class ExtensionMethods
{
    public static string ToBase64String(this byte[] bytes)
        => Convert.ToBase64String(bytes);

    public static byte[] FromBase64String(this string data)
        => Convert.FromBase64String(data);

    public static string GetString(this byte[] data)
        => Encoding.UTF8.GetString(data);

    public static byte[] GetBytes(this string data)
        => Encoding.UTF8.GetBytes(data);
}
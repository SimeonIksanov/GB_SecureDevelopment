namespace JwtSample;

internal class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Enter username");
        string username = Console.ReadLine();

        Console.WriteLine("Enter password");
        string password = Console.ReadLine();

        string token = new UserService().Authenticate(username, password);

        Console.WriteLine(token);
        Console.ReadKey(true);
    }
}
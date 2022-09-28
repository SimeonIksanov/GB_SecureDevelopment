namespace DataProtector;

public class ConnectionString
{
    public string Host { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
    public string DatabaseName { get; set; }

    public override string? ToString()
    {
        return $"Host: {Host}\nDatabaseName: {DatabaseName}\nUserName: {UserName}\nPassword: {Password}";
    }
}

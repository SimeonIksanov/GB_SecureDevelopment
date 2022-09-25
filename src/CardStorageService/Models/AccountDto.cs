namespace CardStorageService.Models;

public class AccountDto
{
    public int Id { get; set; }

    public string Email { get; set; }

    public bool Locked { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string SecondName { get; set; }
}

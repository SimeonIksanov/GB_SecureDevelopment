using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CardStorageService.Data;

[Table("Accounts")]
public class Account : IEntity<int>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [StringLength(255)]
    public string Email { get; set; }

    [StringLength(100)]
    public string PasswordSalt { get; set; }

    [StringLength(100)]
    public string PasswordHash { get; set; }

    public bool Locker { get; set; }

    [StringLength(255)]
    public string FirstName { get; set; }

    [StringLength(255)]
    public string LastName { get; set; }

    [StringLength(255)]
    public string SecondName { get; set; }

    [InverseProperty(nameof(AccountSession.Account))]
    public virtual ICollection<AccountSession> Sessions { get; set; } = new HashSet<AccountSession>();
}

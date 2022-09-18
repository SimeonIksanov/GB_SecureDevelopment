using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace CardStorageService.Data;

[Table("Clients")]
public class Client : IEntity<int>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column, StringLength(255)]
    public string? Surname { get; set; }

    [Column, StringLength(255)]
    public string? FirstName { get; set; }

    [Column, StringLength(255)]
    public string? Patronymic { get; set; }

    [InverseProperty(nameof(Card.Client))]
    public virtual ICollection<Card> Cards { get; set; } = new HashSet<Card>();
}
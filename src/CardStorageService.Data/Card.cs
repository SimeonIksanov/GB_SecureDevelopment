using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace CardStorageService.Data;

[Table("Cards")]
public class Card : IEntity<Guid>
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; }

    [ForeignKey(nameof(Client))]
    public int ClientId { get; set; }

    [Column, StringLength(20)]
    public string CardNo { get; set; }

    [Column, StringLength(50)]
    public string? Name { get; set; }

    [Column, StringLength(50)]
    public string? CVV2 { get; set; }

    [Column]
    public DateTime ExpireDate { get; set; }

    public virtual Client Client { get; set; }
}
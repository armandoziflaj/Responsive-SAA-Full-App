using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saa_Project_BackEnd.Models;

public class UserContact : CommonData
{
    [Required]
    public long UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;

    [Required]
    public long ContactId { get; set; }
    [ForeignKey(nameof(ContactId))]
    public virtual User Contact { get; set; } = null!;
}
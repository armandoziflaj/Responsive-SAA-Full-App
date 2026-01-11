using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saa_Project_BackEnd.Models;

public class Notification : CommonData
{
    [Required]
    [MaxLength(1000)]
    public required string Message { get; set; }
    public bool IsRead { get; set; } = false;
    [MaxLength(45)]
    public string Type { get; set; } = "Alert";
    
    [Required]
    public long UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    
    public long? RelatedId { get; set; }

}
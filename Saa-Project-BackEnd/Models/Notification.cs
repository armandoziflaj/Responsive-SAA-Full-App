using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saa_Project_BackEnd.Models;

public class Notification : CommonData
{
    [Required]
    public required string Message { get; set; }
    public bool IsRead { get; set; } = false;
    public string Type { get; set; } = "Alert";
    
    [Required]
    public long UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
}
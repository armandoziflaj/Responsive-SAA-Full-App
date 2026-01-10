using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saa_Project_BackEnd.Models;

public class GroupMember : CommonData
{
    [Required]
    public long GroupId { get; set; }
    [ForeignKey(nameof(GroupId))]
    public virtual Group Group { get; set; } = null!;
    
    [Required]
    public long UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public virtual User User { get; set; } = null!;
    public bool IsAdmin { get; set; }
}
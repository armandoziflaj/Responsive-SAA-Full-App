using System.ComponentModel.DataAnnotations;

namespace Saa_Project_BackEnd.Models;

public class Group : CommonData
{
    [Required]
    [MaxLength(50)]
    public required string Name { get; set; }
    public string? Description { get; set; }
    public virtual ICollection<GroupMember> Members { get; set; } = [];
}
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Saa_Project_BackEnd.Models;

public class User : IdentityUser<long>
{
    [MaxLength(50)]
    public string? Interests { get; set; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public  DateTime? ModifiedOn { get; set; } = DateTime.UtcNow;
    
    public virtual ICollection<Post> Posts { get; set; } = [];
    public virtual ICollection<UserContact> Contacts { get; set; } = [];
    public virtual ICollection<GroupMember> Memberships { get; set; } = [];
}
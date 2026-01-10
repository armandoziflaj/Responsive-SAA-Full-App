using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saa_Project_BackEnd.Models;

public class Post : CommonData
{
    [Required]
    public required string Title { get; set; }

    [Required]
    public required string Content { get; set; }

    [StringLength(30)]
    public string Category { get; set; } = "General";
    
    [Required]
    public long AuthorId { get; set; }
    [ForeignKey(nameof(AuthorId))]
    public virtual User Author { get; set; } = null!;
}
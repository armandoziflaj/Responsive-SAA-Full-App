using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Saa_Project_BackEnd.Models;

public class ChatMessage : CommonData
{
    [MaxLength(1000)]
    public required string Content { get; set; }
    
    [Required]
    public long SenderId { get; set; }
    [ForeignKey(nameof(SenderId))]
    public virtual User Sender { get; set; } = null!;
    
    public long? ReceiverId { get; set; }
    [ForeignKey(nameof(ReceiverId))]
    public virtual User? Receiver { get; set; }
    
    public long? GroupId { get; set; }
    [ForeignKey(nameof(GroupId))]
    public virtual Group? Group { get; set; }
}
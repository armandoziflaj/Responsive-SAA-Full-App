using System.ComponentModel.DataAnnotations;

namespace Saa_Project_BackEnd.Models;

public class CommonData
{
    [Key]
    public long Id { get; init; }
    public DateTime CreatedOn { get; init; } = DateTime.UtcNow;
    public DateTime ModifiedOn { get; set; } = DateTime.UtcNow;
}
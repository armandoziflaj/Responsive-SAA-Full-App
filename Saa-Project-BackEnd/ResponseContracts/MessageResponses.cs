namespace Saa_Project_BackEnd.ResponseContracts;

public class MessageResponse
{
    public long Id { get; set; }
    public string Content { get; set; } = null!;
    public long SenderId { get; set; }
    public DateTime CreatedOn { get; set; }
}
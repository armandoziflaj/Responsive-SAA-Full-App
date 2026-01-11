namespace Saa_Project_BackEnd.ResponseContracts;

public class NotificationResponse
{
    public long Id { get; set; }
    public string Message { get; set; } = null!;
    public string Type { get; set; } = null!; 
    public bool IsRead { get; set; }
    public DateTime CreatedOn { get; set; }
    
}
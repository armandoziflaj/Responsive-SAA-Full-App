namespace Saa_Project_BackEnd.RequestContracts;

public class SendGroupMessageRequest
{
    public long GroupId { get; set; }
    public required string Content { get; set; }
}
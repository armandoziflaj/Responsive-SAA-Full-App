namespace Saa_Project_BackEnd.RequestContracts;

public class MessagePrivateRequest
{
    public required string Content { get; set; }
    public long ReceiverId { get; set; }
}

public class MessageGroupRequest
{
    public required string Content { get; set; }
    public long GroupId { get; set; }
}
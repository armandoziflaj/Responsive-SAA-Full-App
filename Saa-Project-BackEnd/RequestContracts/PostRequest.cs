namespace Saa_Project_BackEnd.RequestContracts;

public class PostRequest
{
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string Category { get; set; } = "General";
}

public class UpdatePostRequest
{
    public long Id { get; set; }
    public required string Title { get; set; }
    public required string Content { get; set; }
    public string Category { get; set; } = "General";
}
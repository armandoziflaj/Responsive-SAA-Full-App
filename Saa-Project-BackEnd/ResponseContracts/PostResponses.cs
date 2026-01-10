namespace Saa_Project_BackEnd.ResponseContracts;

public class PostResponse
{
    public long Id { get; set; }
    public string Title { get; set; } = null!;
    public string Content { get; set; } = null!;
    public string Category { get; set; } = null!;
    public string AuthorName { get; set; } = null!;
    public DateTime CreatedOn { get; set; }
}

public class PostIdResponse
{
    public long Id { get; set; }
}

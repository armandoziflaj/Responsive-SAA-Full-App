namespace Saa_Project_BackEnd.RequestContracts;

public class GroupUpdateRequest
{
    public long Id { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
}

public class GroupPostRequest
{
    public required string Name { get; set; }
    public string? Description { get; set; }
}
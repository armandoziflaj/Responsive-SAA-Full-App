namespace Saa_Project_BackEnd.ResponseContracts;

public class GroupGetResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public int MemberCount { get; set; }
    public bool IsAdmin { get; set; }
}

public class GroupIdResponse
{
    public long Id { get; set; }
}
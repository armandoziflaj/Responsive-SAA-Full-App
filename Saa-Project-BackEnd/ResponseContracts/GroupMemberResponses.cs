namespace Saa_Project_BackEnd.RequestContracts;

public class GroupMemberResponse
{
    public string UserName { get; set; } = null!;
    public string GroupName { get; set; } = null!;
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class UpdateMemberResponse
{
    public long Id { get; set; }
    public bool IsAdmin { get; set; }
}

public class GetGroupMember
{
    public long Id { get; set; }
    public bool IsAdmin { get; set; }
    public DateTime JoinedAt { get; set; }
}


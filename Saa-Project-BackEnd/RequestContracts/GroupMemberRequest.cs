namespace Saa_Project_BackEnd.ResponseContracts;

public class GroupMemberRequest
{
    public long GroupId { get; set; }
    public long UserId { get; set; }
}

public class UpdateGroupMember
{
    public long GroupId { get; set; }
    public long UserId { get; set; }
    public bool IsAdmin { get; set; }
}
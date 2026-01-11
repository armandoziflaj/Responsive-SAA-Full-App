using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saa_Project_BackEnd.Data;
using Saa_Project_BackEnd.Models;
using Saa_Project_BackEnd.RequestContracts;
using Saa_Project_BackEnd.ResponseContracts;

namespace Saa_Project_BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupMembersController(AppDbContext context) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<BaseResponse<GroupMemberResponse>>> AddMember(GroupMemberRequest request)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        if (currentUserId != request.UserId)
        {
            var isAdmin = await context.GroupMembers
                .AnyAsync(m => m.GroupId == request.GroupId && m.UserId == currentUserId && m.IsAdmin);

            if (!isAdmin)
            {
                return Ok(BaseResponse<GroupMemberResponse>.Failure(["User is not Admin in this group"]));
            }
        }

        var isAlreadyMember = await context.GroupMembers
            .AnyAsync(m => m.GroupId == request.GroupId && m.UserId == request.UserId);
    
        if (isAlreadyMember) 
            return BadRequest(BaseResponse<GroupMemberResponse>.Failure(["User is already in this group"]));

        var member = new GroupMember
        {
            GroupId = request.GroupId,
            UserId = request.UserId,
            IsAdmin = false,
        };

        var groupName = await context.Groups.Where(y => y.Id == request.GroupId).Select(x => x.Name).FirstOrDefaultAsync();
        var userName = await context.Users.Where(y => y.Id == request.UserId).Select(x => x.UserName).FirstOrDefaultAsync();
        
        var response = new GroupMemberResponse
        {
            UserName = userName ?? "User",
            GroupName = groupName ?? "Group",
            IsAdmin = member.IsAdmin,
            JoinedAt = member.CreatedOn
        };
        context.GroupMembers.Add(member);
        await context.SaveChangesAsync();
        
        return Ok(BaseResponse<GroupMemberResponse>.Success(response));
    }

    [HttpDelete("{groupId}/{userId}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteMember(long groupId, long userId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var member = await context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);

        if (member == null)
            return NotFound(BaseResponse<string>.Failure(["User is not member of this group chat."]));
        
        if (currentUserId != userId)
        {
            var isCurrentAdmin = await context.GroupMembers
                .AnyAsync(m => m.GroupId == groupId && m.UserId == currentUserId && m.IsAdmin);

            if (!isCurrentAdmin)
            {
                return Ok(BaseResponse<string>.Failure(["Only admins can remove group members"]));
            }
        }

        context.GroupMembers.Remove(member);
        await context.SaveChangesAsync();

        return Ok(BaseResponse<string>.Success("Group member has been removed."));
    }

    [HttpPut]
    public async Task<ActionResult<BaseResponse<UpdateMemberResponse>>> UpdateMember(UpdateGroupMember request)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var currentUserMembership = await context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == request.GroupId && m.UserId == currentUserId);

        if (currentUserMembership == null || !currentUserMembership.IsAdmin)
        {
            return Ok(BaseResponse<UpdateMemberResponse>.Failure(["Only admins can make changes to group members."]));
        }

        var memberToUpdate = await context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == request.GroupId && m.UserId == request.UserId);
        if (memberToUpdate == null)
        {
            return NotFound(BaseResponse<UpdateMemberResponse>.Failure(["User is not member of this group chat."]));
        }
        memberToUpdate.IsAdmin = request.IsAdmin;
        memberToUpdate.ModifiedOn = DateTime.UtcNow;
        await context.SaveChangesAsync();

        var response = new UpdateMemberResponse
        {
            Id = request.UserId,
            IsAdmin = request.IsAdmin
        };

        return Ok(BaseResponse<UpdateMemberResponse>.Success(response));
    }

    [HttpGet("{groupId}/{userId}")]
    public async Task<ActionResult<BaseResponse<GetGroupMember>>> GetMember(long groupId, long userId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var requesterIsMember = await context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == currentUserId);

        if (!requesterIsMember && currentUserId != userId)
        {
            return Ok(BaseResponse<GetGroupMember>.Failure(["User is not member of this group chat."]));
        }
        
        var member = await context.GroupMembers
            .FirstOrDefaultAsync(m => m.GroupId == groupId && m.UserId == userId);
        
        if (member == null)
            return Ok(BaseResponse<GetGroupMember>.Failure(["Did not find a the User in this GroupChat. "]));

        var response = new GetGroupMember()
        {
            Id = member.Id,
            IsAdmin = member.IsAdmin,
            JoinedAt = member.CreatedOn
        };
        
        return Ok(BaseResponse<GetGroupMember>.Success(response));
    }
    
    [HttpGet("{groupId}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<GetGroupMember>>>> GetMembers(long groupId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    
        var requesterIsMember = await context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == currentUserId);

        if (!requesterIsMember)
        {
            return Ok(BaseResponse<IEnumerable<GetGroupMember>>.Failure(["You are not a member of this group chat."]));
        }
    
        var members = await context.GroupMembers
            .Where(m => m.GroupId == groupId)
            .Include(m => m.User) 
            .Select(m => new GetGroupMember
            {
                Id = m.UserId,
                IsAdmin = m.IsAdmin,
                JoinedAt = m.CreatedOn
            })
            .ToListAsync();
    
        if (members.Count == 0)
            return Ok(BaseResponse<IEnumerable<GetGroupMember>>.Failure(["Did not find users in this GroupChat."]));

        return Ok(BaseResponse<IEnumerable<GetGroupMember>>.Success(members));
    }
}
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
public class GroupController(AppDbContext context) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<BaseResponse<List<GroupGetResponse>>>>
        GetGroups()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var groups = await context.Groups
            .Where(g => g.Members.Any(m => m.UserId == currentUserId))
            .Select(g => new GroupGetResponse
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                MemberCount = g.Members.Count(),
                IsAdmin = g.Members.Any(m => m.UserId == currentUserId && m.IsAdmin)
            })
            .ToListAsync();

        if (groups.Count == 0)
            return Ok(BaseResponse<IEnumerable<GroupGetResponse>>.Failure(["User is not member of a chat group yet."]));

        return Ok(BaseResponse<IEnumerable<GroupGetResponse>>.Success(groups));
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<BaseResponse<GroupGetResponse>>>
        GetGroup(long id)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var group = await context.Groups
            .Where(g => g.Members.Any(m => m.UserId == currentUserId))
            .Select(g => new GroupGetResponse
            {
                Id = g.Id,
                Name = g.Name,
                Description = g.Description,
                MemberCount = g.Members.Count(),
                IsAdmin = g.Members.Any(m => m.UserId == currentUserId && m.IsAdmin)
            })
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
            return Ok(BaseResponse<GroupGetResponse>.Failure(["User is not member of this chat group yet."]));

        return Ok(BaseResponse<GroupGetResponse>.Success(group));
    }

    [HttpPut]
    public async Task<ActionResult<BaseResponse<GroupIdResponse>>>
        UpdateGroup(GroupUpdateRequest request)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var group = await context.Groups
            .Include(g => g.Members)
            .FirstOrDefaultAsync(g => g.Id == request.Id);

        if (group == null)
        {
            return Ok(BaseResponse<GroupIdResponse>.Failure(["Group not found."]));
        }

        var isAdmin = group.Members.Any(m => m.UserId == currentUserId && m.IsAdmin);

        if (!isAdmin)
            return Ok(BaseResponse<GroupIdResponse>.Failure(["Only group admins can update group details."]));

        group.Name = request.Name;
        group.Description = request.Description;
        group.ModifiedOn = DateTime.UtcNow;

        await context.SaveChangesAsync();

        var response = new GroupIdResponse { Id = group.Id };
        return Ok(BaseResponse<GroupIdResponse>.Success(response));
    }
    
    [HttpPost]
    public async Task<ActionResult<BaseResponse<GroupIdResponse>>> SaveGroup(GroupPostRequest request)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        
        var newGroup = new Group
        {
            Name = request.Name,
            Description = request.Description,
            CreatedOn = DateTime.UtcNow 
        };
        
        var adminMember = new GroupMember
        {
            UserId = currentUserId,
            IsAdmin = true,
            CreatedOn = DateTime.UtcNow
        };
        
        newGroup.Members.Add(adminMember);
        
        context.Groups.Add(newGroup);
        await context.SaveChangesAsync();
    
        var response = new GroupIdResponse { Id = newGroup.Id };
        return Ok(BaseResponse<GroupIdResponse>.Success(response));
    }
}
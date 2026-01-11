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
public class MessagesController(AppDbContext context) : ControllerBase
{
    [HttpPost("private")]
    public async Task<ActionResult<BaseResponse<MessageResponse>>> SendPrivateMessage(MessagePrivateRequest request)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var receiverExists = await context.Users.AnyAsync(u => u.Id == request.ReceiverId);
        if (!receiverExists)
            return NotFound(BaseResponse<MessageResponse>.Failure(["Receiver not found."]));

        var newMessage = new ChatMessage
        {
            Content = request.Content,
            SenderId = currentUserId,
            ReceiverId = request.ReceiverId,
            GroupId = null,
            CreatedOn = DateTime.UtcNow
        };

        context.ChatMessages.Add(newMessage);
        
        var notification = new Notification
        {
            UserId = request.ReceiverId,
            Message = $"You have a new message.",
            Type = "PrivateMessage",
            RelatedId = currentUserId,
            IsRead = false,
            CreatedOn = DateTime.UtcNow
        };
        context.Notifications.Add(notification);
        await context.SaveChangesAsync();

        var response = new MessageResponse
        {
            Id = newMessage.Id,
            Content = newMessage.Content,
            SenderId = newMessage.SenderId,
            CreatedOn = newMessage.CreatedOn
        };
        return Ok(BaseResponse<MessageResponse>.Success(response));
    }
    [HttpPost("group")]
    public async Task<ActionResult<BaseResponse<MessageResponse>>> SendGroupMessage(MessageGroupRequest request)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isMember = await context.GroupMembers
            .AnyAsync(m => m.GroupId == request.GroupId && m.UserId == currentUserId);

        if (!isMember)
            return Forbid();

        var newMessage = new ChatMessage
        {
            Content = request.Content,
            SenderId = currentUserId,
            GroupId = request.GroupId,
            ReceiverId = null,
            CreatedOn = DateTime.UtcNow
        };

        context.ChatMessages.Add(newMessage);
        
        var otherMembers = await context.GroupMembers
            .Where(m => m.GroupId == request.GroupId && m.UserId != currentUserId)
            .Select(m => m.UserId)
            .ToListAsync();

        foreach (var memberId in otherMembers)
        {
            context.Notifications.Add(new Notification
            {
                UserId = memberId,
                Message = $"Νέο μήνυμα στην ομάδα.",
                Type = "GroupMessage",
                //RelatedId = request.GroupId, 
                IsRead = false,
                CreatedOn = DateTime.UtcNow
            });
        }
        await context.SaveChangesAsync();
        
        var response = new MessageResponse
        {
            Id = newMessage.Id,
            Content = newMessage.Content,
            SenderId = newMessage.SenderId,
            CreatedOn = newMessage.CreatedOn
        };
        
        return Ok(BaseResponse<MessageResponse>.Success(response));
    }
    
    [HttpGet("private/{otherUserId}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<MessageResponse>>>> GetPrivateMessages(long otherUserId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var messages = await context.ChatMessages
            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) || 
                        (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.CreatedOn)
            .Select(m => new MessageResponse
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                CreatedOn = m.CreatedOn
            })
            .ToListAsync();

        return Ok(BaseResponse<IEnumerable<MessageResponse>>.Success(messages));
    }

    [HttpGet("group/{groupId}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<MessageResponse>>>> GetGroupMessages(long groupId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var isMember = await context.GroupMembers
            .AnyAsync(m => m.GroupId == groupId && m.UserId == currentUserId);
        
        if (!isMember) return Forbid();

        var messages = await context.ChatMessages
            .Where(m => m.GroupId == groupId)
            .OrderBy(m => m.CreatedOn)
            .Select(m => new MessageResponse
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                CreatedOn = m.CreatedOn
            })
            .ToListAsync();

        return Ok(BaseResponse<IEnumerable<MessageResponse>>.Success(messages));
    }
}
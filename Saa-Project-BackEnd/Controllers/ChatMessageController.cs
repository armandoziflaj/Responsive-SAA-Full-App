using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Saa_Project_BackEnd.Data;
using Saa_Project_BackEnd.Hubs;
using Saa_Project_BackEnd.Models;
using Saa_Project_BackEnd.RequestContracts;
using Saa_Project_BackEnd.ResponseContracts;

namespace Saa_Project_BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class MessagesController(AppDbContext context, IHubContext<ChatHub> hubContext) : ControllerBase
{
    
[HttpPost("private")]
public async Task<ActionResult<BaseResponse<MessageResponse>>> SendPrivateMessage(MessagePrivateRequest request)
{
    var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    if (currentUserId == request.ReceiverId)
        return BadRequest(BaseResponse<MessageResponse>.Failure(["You cannot send a message to yourself."]));

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
        Message = $"New message from user {currentUserId}",
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
        ReceiverId = newMessage.ReceiverId,
        SenderId = newMessage.SenderId,
        CreatedOn = newMessage.CreatedOn
    };

    await hubContext.Clients.Group($"private_{request.ReceiverId}").SendAsync("ReceiveNotification", notification);
    
    await hubContext.Clients.Group($"private_{request.ReceiverId}").SendAsync("ReceiveMessage", response);
    await hubContext.Clients.Group($"private_{currentUserId}").SendAsync("ReceiveMessage", response);

    return Ok(BaseResponse<MessageResponse>.Success(response));
}

[HttpPost("group")]
public async Task<ActionResult<BaseResponse<MessageResponse>>> SendGroupMessage(MessageGroupRequest request)
{
    var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    var isMember = await context.GroupMembers
        .AnyAsync(m => m.GroupId == request.GroupId && m.UserId == currentUserId);

    if (!isMember) return Forbid();

    var newMessage = new ChatMessage
    {
        Content = request.Content,
        SenderId = currentUserId,
        GroupId = request.GroupId,
        CreatedOn = DateTime.UtcNow
    };

    context.ChatMessages.Add(newMessage);
    await context.SaveChangesAsync();
    
    var response = new MessageResponse
    {
        Id = newMessage.Id, 
        Content = newMessage.Content,
        SenderId = newMessage.SenderId,
        GroupId = newMessage.GroupId,
        CreatedOn = newMessage.CreatedOn
    };

    var otherMembers = await context.GroupMembers
        .Where(m => m.GroupId == request.GroupId && m.UserId != currentUserId)
        .Select(m => m.UserId)
        .Distinct()
        .ToListAsync();

    var notificationsToSend = new List<Notification>();

    foreach (var memberId in otherMembers)
    {
        var notif = new Notification
        {
            UserId = memberId,
            Message = $"New message in group {request.GroupId}.",
            Type = "GroupMessage",
            RelatedId = currentUserId, 
            IsRead = false,
            CreatedOn = DateTime.UtcNow
        };
        context.Notifications.Add(notif);
        notificationsToSend.Add(notif);
    }

    await context.SaveChangesAsync(); 

    foreach (var notif in notificationsToSend)
    {
        await hubContext.Clients.Group($"private_{notif.UserId}")
            .SendAsync("ReceiveNotification", notif);
    }

    await hubContext.Clients.Group($"group_{request.GroupId}")
        .SendAsync("ReceiveMessage", response);

    return Ok(BaseResponse<MessageResponse>.Success(response));
}
    
    [HttpGet("private/{otherUserId}")]
    public async Task<ActionResult<BaseResponse<IEnumerable<MessageResponse>>>> GetPrivateMessages(long otherUserId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var messages = await context.ChatMessages
            .AsNoTracking()
            .Where(m => (m.SenderId == currentUserId && m.ReceiverId == otherUserId) || 
                        (m.SenderId == otherUserId && m.ReceiverId == currentUserId))
            .OrderBy(m => m.CreatedOn)
            .Select(m => new MessageResponse
            {
                Id = m.Id,
                Content = m.Content,
                SenderId = m.SenderId,
                ReceiverId = m.ReceiverId,
                CreatedOn = m.CreatedOn,
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
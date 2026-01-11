using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saa_Project_BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Saa_Project_BackEnd.ResponseContracts;

namespace Saa_Project_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class NotificationsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<BaseResponse<IEnumerable<NotificationResponse>>>> GetNotifications()
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var notifications = await context.Notifications
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedOn)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedOn = n.CreatedOn,
                })
                .ToListAsync();

            return Ok(BaseResponse<IEnumerable<NotificationResponse>>.Success(notifications));
        }

        [HttpGet("unread-count")]
        public async Task<ActionResult<BaseResponse<int>>> GetUnreadCount()
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var count = await context.Notifications
                .CountAsync(n => n.UserId == userId && !n.IsRead);

            return Ok(BaseResponse<int>.Success(count));
        }

        [HttpPatch("{id}/read")]
        public async Task<ActionResult<BaseResponse<bool>>> MarkAsRead(long id)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            
            var notification = await context.Notifications
                .FirstOrDefaultAsync(n => n.Id == id && n.UserId == userId);

            if (notification == null) 
                return NotFound(BaseResponse<bool>.Failure(["Notification not found"]));

            notification.IsRead = true;
            await context.SaveChangesAsync();

            return Ok(BaseResponse<bool>.Success(true));
        }

        [HttpPatch("read-all")]
        public async Task<ActionResult<BaseResponse<bool>>> MarkAllAsRead()
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var unreadNotifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            foreach (var notification in unreadNotifications)
            {
                notification.IsRead = true;
            }

            await context.SaveChangesAsync();
            return Ok(BaseResponse<bool>.Success(true));
        }
    }
}
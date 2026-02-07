using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saa_Project_BackEnd.Data;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Saa_Project_BackEnd.ResponseContracts;

namespace Saa_Project_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},Identity.Bearer")]
    public class NotificationsController(AppDbContext context) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<BaseResponse<IEnumerable<NotificationResponse>>>> GetNotifications()
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                    ?? User.FindFirstValue("sub") 
                                    ?? User.FindFirstValue("id")!);

            var notifications = await context.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .OrderByDescending(n => n.CreatedOn)
                .Select(n => new NotificationResponse
                {
                    Id = n.Id,
                    Message = n.Message,
                    Type = n.Type,
                    IsRead = n.IsRead,
                    CreatedOn = n.CreatedOn,
                    RelatedId = n.RelatedId
                })
                .ToListAsync();

            return Ok(BaseResponse<IEnumerable<NotificationResponse>>.Success(notifications));
        }

        [HttpPatch("{id}/read")]
        public async Task<ActionResult<BaseResponse<bool>>> MarkAsRead(long id)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                    ?? User.FindFirstValue("sub") 
                                    ?? User.FindFirstValue("id")!);
            
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
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                    ?? User.FindFirstValue("sub") 
                                    ?? User.FindFirstValue("id")!);

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
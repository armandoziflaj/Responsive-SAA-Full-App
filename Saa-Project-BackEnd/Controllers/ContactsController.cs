using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saa_Project_BackEnd.Data;
using Saa_Project_BackEnd.Models;
using Saa_Project_BackEnd.ResponseContracts;

namespace Saa_Project_BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},Identity.Bearer")]
public class ContactsController(AppDbContext context) : ControllerBase
{
    [HttpPost("{contactId}")]
    public async Task<ActionResult<BaseResponse<string>>> AddContact(long contactId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                       ?? User.FindFirstValue("sub") 
                                       ?? User.FindFirstValue("id")!);

        if (currentUserId == contactId)
            return Ok(BaseResponse<string>.Failure(["You can't add yourself."]));

        var alreadyExists = await context.UserContacts
            .AnyAsync(c => c.UserId == currentUserId && c.ContactId == contactId);

        if (alreadyExists)
            return Ok(BaseResponse<string>.Failure(["User is already in the contact list."]));

        var contactExists = await context.Users.AnyAsync(u => u.Id == contactId);
        if (!contactExists)
            return NotFound(BaseResponse<string>.Failure(["User not found."]));

        var newContact = new UserContact
        {
            UserId = currentUserId,
            ContactId = contactId,
            CreatedOn = DateTime.UtcNow
        };

        context.UserContacts.Add(newContact);
        await context.SaveChangesAsync();

        return Ok(BaseResponse<string>.Success("Contact has been added successfully!"));
    }

    [HttpDelete("{contactId}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteContact(long contactId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                       ?? User.FindFirstValue("sub") 
                                       ?? User.FindFirstValue("id")!);

        var contactRecord = await context.UserContacts
            .FirstOrDefaultAsync(c => c.UserId == currentUserId && c.ContactId == contactId);

        if (contactRecord == null)
            return NotFound(BaseResponse<string>.Failure(["Contact not found."]));

        context.UserContacts.Remove(contactRecord);
        await context.SaveChangesAsync();

        return Ok(BaseResponse<string>.Success("Contact has been deleted successfully."));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<IEnumerable<ContactResponse>>>> GetMyContacts()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                       ?? User.FindFirstValue("sub") 
                                       ?? User.FindFirstValue("id")!);
        
        var contacts = await context.UserContacts
            .Where(c => c.UserId == currentUserId || c.ContactId == currentUserId)
            .Include(c => c.Contact) 
            .Select(c => new ContactResponse
            {
                ContactId = c.UserId == currentUserId ? c.ContactId : c.UserId,
                UserName = c.UserId == currentUserId 
                    ? (c.Contact.UserName ?? "Unknown") 
                    : (c.User.UserName ?? "Unknown"),
                
                Interests = c.UserId == currentUserId 
                    ? (c.Contact.Interests ?? "") 
                    : (c.User.Interests ?? ""),
                
                AddedOn = c.CreatedOn
            })
            .ToListAsync();

        return Ok(BaseResponse<IEnumerable<ContactResponse>>.Success(contacts));
    }
}
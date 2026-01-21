using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saa_Project_BackEnd.Data;
using Saa_Project_BackEnd.Models;
using Saa_Project_BackEnd.ResponseContracts;

namespace Saa_Project_BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ContactsController(AppDbContext context) : ControllerBase
{
    [HttpPost("{contactId}")]
    public async Task<ActionResult<BaseResponse<string>>> AddContact(long contactId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

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

        return Ok(BaseResponse<string>.Success("Contact has been added succe!"));
    }

    [HttpDelete("{contactId}")]
    public async Task<ActionResult<BaseResponse<string>>> DeleteContact(long contactId)
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var contactRecord = await context.UserContacts
            .FirstOrDefaultAsync(c => c.UserId == currentUserId && c.ContactId == contactId);

        if (contactRecord == null)
            return NotFound(BaseResponse<string>.Failure(["Η επαφή δεν βρέθηκε στη λίστα σας."]));

        context.UserContacts.Remove(contactRecord);
        await context.SaveChangesAsync();

        return Ok(BaseResponse<string>.Success("Η επαφή αφαιρέθηκε."));
    }

    [HttpGet]
    public async Task<ActionResult<BaseResponse<IEnumerable<ContactResponse>>>> GetMyContacts()
    {
        var currentUserId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        var contacts = await context.UserContacts
            .Where(c => c.UserId == currentUserId)
            .Include(c => c.Contact) // Φέρνουμε τα στοιχεία του χρήστη-επαφή
            .Select(c => new ContactResponse
            {
                ContactId = c.ContactId,
                UserName = c.Contact.UserName ?? "Unknown",
                Interests = c.Contact.Interests ?? "",
                AddedOn = c.CreatedOn
            })
            .ToListAsync();

        return Ok(BaseResponse<IEnumerable<ContactResponse>>.Success(contacts));
    }
}
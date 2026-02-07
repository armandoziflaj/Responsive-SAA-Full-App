using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Saa_Project_BackEnd.Data;
using Saa_Project_BackEnd.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Saa_Project_BackEnd.RequestContracts;
using Saa_Project_BackEnd.ResponseContracts;

namespace Saa_Project_BackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},Identity.Bearer")]
    public class PostsController(AppDbContext context) : ControllerBase
    {
        
        [HttpGet]
        [AllowAnonymous] 
        public async Task<ActionResult<BaseResponse<IEnumerable<PostResponse>>>> 
            GetPosts([FromQuery] string? category)
        {
            var query = context.Posts
                .Include(p => p.Author)
                .AsQueryable();

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(p => p.Category.ToLower() == category.ToLower());
            }

            var posts = await query
                .OrderByDescending(x => x.CreatedOn)
                .Select(p => new PostResponse
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Category = p.Category,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.UserName ?? "User",
                    CreatedOn = p.CreatedOn
                })
                .ToListAsync();
            
            return Ok(BaseResponse<IEnumerable<PostResponse>>.Success(posts));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<BaseResponse<PostResponse>>> GetPost(long id)
        {
            var post = await context.Posts
                .Include(p => p.Author)
                .Select(p=> new PostResponse()
                {
                    Id = p.Id,
                    Title = p.Title,
                    Content = p.Content,
                    Category = p.Category,
                    AuthorId = p.AuthorId,
                    AuthorName = p.Author.UserName ?? "User",
                    CreatedOn = p.CreatedOn
                })
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return NotFound();

            return Ok(BaseResponse<PostResponse>.Success(post));
        }

        [HttpPost]
        public async Task<ActionResult<BaseResponse<PostIdResponse>>> PostPost(PostRequest request)
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (userIdClaim == null) return Unauthorized();

            var post = new Post
            {
                Title = request.Title,
                Content = request.Content,
                Category = request.Category,
                AuthorId = long.Parse(userIdClaim),
                CreatedOn = DateTime.UtcNow
            };

            context.Posts.Add(post);
            await context.SaveChangesAsync();

            return Ok(BaseResponse<PostIdResponse>.Success(new PostIdResponse { Id = post.Id }));
        }

        [HttpPut]
        public async Task<ActionResult<BaseResponse<PostIdResponse>>> PutPost(UpdatePostRequest request)
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) 
                                    ?? User.FindFirstValue("sub") 
                                    ?? User.FindFirstValue("id")!);
            
            var post = await context.Posts.FindAsync(request.Id);

            if (post == null)
                return NotFound(BaseResponse<PostIdResponse>.Failure(["Post not found"]));
            
            if (post.AuthorId != userId)
            {
                return Forbid();
            }
            
            post.Title = request.Title;
            post.Content = request.Content;
            post.Category = request.Category; 
            post.ModifiedOn = DateTime.UtcNow;

            await context.SaveChangesAsync();

            return Ok(BaseResponse<PostIdResponse>.Success(new PostIdResponse { Id = post.Id }));
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<BaseResponse<PostIdResponse>>> DeletePost(long id)
        {
            var post = await context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (post.AuthorId != userId)
            {
                return Forbid();
            }

            context.Posts.Remove(post);
            await context.SaveChangesAsync();

            var response = new PostIdResponse { Id = post.Id };
            
            return Ok(BaseResponse<PostIdResponse>.Success(response));
        }

        /*private bool PostExists(long id)
        {
            return context.Posts.Any(e => e.Id == id);
        }*/
    }
}
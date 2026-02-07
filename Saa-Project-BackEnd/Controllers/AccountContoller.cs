using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Saa_Project_BackEnd.Data;
using Saa_Project_BackEnd.ResponseContracts;

namespace Saa_Project_BackEnd.Controllers;
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController
        : ControllerBase
    {
        [HttpGet("GetCurrentUser")]
        [Authorize(AuthenticationSchemes = $"{JwtBearerDefaults.AuthenticationScheme},Identity.Bearer")]
        public ActionResult<BaseResponse<LoginResponse>> GetCurrentUser()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                               ?? User.FindFirstValue("sub") 
                               ?? User.FindFirstValue("id");

            if (string.IsNullOrEmpty(userIdString))
            {
                return Unauthorized(BaseResponse<LoginResponse>.Failure([$"User not found."]));
            }
    
            var response = new LoginResponse 
            { 
                UserId = userIdString,
                Username = User.FindFirstValue(ClaimTypes.Name)
            };

            return Ok(BaseResponse<LoginResponse>.Success(response));
        }
    }
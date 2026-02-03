using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Saa_Project_BackEnd.Models;

namespace Saa_Project_BackEnd.Controllers;

[Route("api/[controller]")]
[ApiController]
public class GoogleController(
    UserManager<User> userManager, 
    IConfiguration configuration) : ControllerBase 
{
    [HttpGet("login")]
    [AllowAnonymous]
    public IActionResult Login()
    {
        var redirectUrl = Url.Action("GoogleResponse", "Google");
        
        var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        
        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    [AllowAnonymous]
    public async Task<IActionResult> GoogleResponse()
    {
        var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
        if (!result.Succeeded) return BadRequest("Google authentication failed.");

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        if (string.IsNullOrEmpty(email)) return BadRequest("Email not received.");

        var user = await userManager.FindByEmailAsync(email);
        if (user == null)
        {
            user = new User { Email = email, UserName = email, EmailConfirmed = true };
            await userManager.CreateAsync(user);
            var info = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);
            await userManager.AddLoginAsync(user, new UserLoginInfo("Google", info.Principal?.FindFirstValue(ClaimTypes.NameIdentifier)!, "Google"));
        }

        var token = GenerateJwtToken(user); 
    
        await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
        return Redirect($"http://localhost:5173/login-success?token={token}");
    }

    private string GenerateJwtToken(User user)
    {
        var jwtKey = configuration["Jwt:Key"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey!));
    
        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), 
            new Claim(ClaimTypes.Name, user.Email!), 
            new Claim(JwtRegisteredClaimNames.Email, user.Email!),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        };

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["Jwt:Issuer"],
            audience: configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
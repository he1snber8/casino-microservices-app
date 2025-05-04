using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CasinoGame.API.Models;
using CasinoGame.Facade.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace CasinoGame.API.Controllers;

public class AuthController(IOptions<JwtSettings> jwtSettings) : Controller
{
    
    [HttpPost("login")]
    public  IActionResult LogIn([FromBody] LoginRequest loginRequest)
    {

        var users = new Dictionary<string, string>
        {
            { "player1", "password1" },
            { "player2", "password2" }
        };

        if (!users.TryGetValue(loginRequest.Username, out var storedPassword) || storedPassword != loginRequest.Password)
        {
            return Unauthorized("Invalid credentials.");
        }
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(jwtSettings.Value.Key);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, loginRequest.Username),
            new Claim("client", "LobbyService") 
        };

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(60),
            Issuer = jwtSettings.Value.Issuer,
            Audience = jwtSettings.Value.Audience,
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return Ok(new { token = tokenHandler.WriteToken(token) });
    }
}
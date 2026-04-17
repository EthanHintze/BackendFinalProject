using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using backendfinal.Data;
using backendfinal.Dtos;
using backendfinal.Entities;
using backendfinal.Exceptions;

namespace backendfinal.Services;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}

public class AuthService(AppDbContext db, IConfiguration config) : IAuthService
{
    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        var user = await db.Users
            .SingleOrDefaultAsync(u => u.Username == request.Username)
            ?? throw new NotFoundException($"User '{request.Username}' not found.");

        if (!BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            throw new BusinessRuleException("Invalid credentials.");

        return new LoginResponse(GenerateToken(user), user.Username);
    }

    private string GenerateToken(User user)
    {
        var key   = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        };

        var token = new JwtSecurityToken(
            issuer:             config["Jwt:Issuer"],
            audience:           config["Jwt:Audience"],
            claims:             claims,
            expires:            DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
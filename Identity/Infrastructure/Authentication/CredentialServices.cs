using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CeTe.Identity.Application.Services;
using CeTe.Identity.Domain.Model.Aggregates;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
namespace CeTe.Identity.Infrastructure.Authentication;

public sealed class PasswordService : IPasswordService
{
    private readonly PasswordHasher<User> hasher = new();
    public string Hash(User user, string password) => hasher.HashPassword(user, password);
    public bool Verify(User user, string password) =>
        hasher.VerifyHashedPassword(user, user.PasswordHash, password) != PasswordVerificationResult.Failed;
}
public sealed class TokenService(IConfiguration configuration) : ITokenService
{
    public string Issue(User user, UserSession session)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("tenant_id", user.TenantId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, session.Id.ToString())
        };
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)),
            SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken("CeTe", "CeTe", claims,
            expires: session.ExpiresAt.UtcDateTime, signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

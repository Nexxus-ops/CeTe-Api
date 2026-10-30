using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CeTe.Application;
using CeTe.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CeTe.Infrastructure;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid TenantId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue("tenant_id"), out var id) ? id : Guid.Empty;
    public Guid UserId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
    public string Username => accessor.HttpContext?.User.Identity?.Name ?? "";
}
public record RegisterInput(string Username, string Password, string CompanyName, string Ruc, string Address);
public record LoginInput(string Username, string Password);
public record EmployeeInput(string Username, string Password, string Role);
public sealed class Authentication(CeTeDb db, ICurrentUser current, IConfiguration config)
{
    private static readonly PasswordHasher<User> Hasher = new();
    private static string Email(string value)
    {
        var normalized = value.Trim().ToLowerInvariant();
        Rules.Require(System.Net.Mail.MailAddress.TryCreate(normalized, out var address) && address.Address == normalized, "Enter a valid email address.");
        return normalized;
    }
    private static void Password(string password) => Rules.Require(password.Length is >= 10 and <= 128, "Password must contain between 10 and 128 characters.");
    public async Task<object> Register(RegisterInput input)
    {
        var email = Email(input.Username);
        Password(input.Password);
        Rules.Require(!string.IsNullOrWhiteSpace(input.CompanyName) && !string.IsNullOrWhiteSpace(input.Address) && input.Ruc.Length == 11 && input.Ruc.All(char.IsAsciiDigit), "Company, address and an 11-digit RUC are required.");
        Rules.Require(!await db.Set<User>().IgnoreQueryFilters().AnyAsync(u => u.Username == email) && !await db.Set<Tenant>().AnyAsync(t => t.Ruc == input.Ruc), "The account or company is already registered.", 409);
        var tenant = new Tenant { Name = input.CompanyName.Trim(), Ruc = input.Ruc, Address = input.Address.Trim() };
        var user = new User { TenantId = tenant.Id, Username = email, Role = "Admin" };
        user.PasswordHash = Hasher.HashPassword(user, input.Password);
        await db.Transaction(async () => { db.Add(tenant); db.Add(user); db.Add(new Warehouse { TenantId = tenant.Id, Name = "Main warehouse", Address = tenant.Address }); foreach (var name in new[] { "Admin", "Warehouse", "Sales", "Logistics" }) { var role = new Role { TenantId = tenant.Id, Name = name }; db.Add(role); if (name == "Admin") db.Add(new UserRole { TenantId = tenant.Id, UserId = user.Id, RoleId = role.Id }); } return true; });
        return new
        {
            user.Id,
            user.Username,
            tenantId = tenant.Id
        };
    }
    public async Task<object> Login(LoginInput input)
    {
        var email = input.Username.Trim().ToLowerInvariant();
        var user = await db.Set<User>().IgnoreQueryFilters().SingleOrDefaultAsync(u => u.Username == email && u.Active);
        if (user == null || Hasher.VerifyHashedPassword(user, user.PasswordHash, input.Password) == PasswordVerificationResult.Failed)
            throw new BusinessException("Invalid credentials.", 401);
        var session = new UserSession { TenantId = user.TenantId, UserId = user.Id, ExpiresAt = DateTimeOffset.UtcNow.AddHours(4) };
        db.Add(session);
        await db.SaveChangesAsync();
        var claims = new[] { new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()), new Claim(ClaimTypes.Name, user.Username), new Claim(ClaimTypes.Role, user.Role), new Claim("tenant_id", user.TenantId.ToString()), new Claim(JwtRegisteredClaimNames.Jti, session.Id.ToString()) };
        var jwt = new JwtSecurityToken("CeTe", "CeTe", claims, expires: session.ExpiresAt.UtcDateTime, signingCredentials: new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]!)), SecurityAlgorithms.HmacSha256));
        return new
        {
            user.Id,
            user.Username,
            user.Role,
            tenantId = user.TenantId,
            token = new JwtSecurityTokenHandler().WriteToken(jwt),
            expiresAt = session.ExpiresAt
        };
    }
    public async Task Logout(ClaimsPrincipal principal)
    {
        var id = Guid.Parse(principal.FindFirstValue(JwtRegisteredClaimNames.Jti)!);
        var session = await db.Find<UserSession>(id);
        if (session != null)
        {
            session.Revoked = true;
            await db.SaveChangesAsync();
        }
    }
    public async Task<object> Employee(EmployeeInput input)
    {
        var email = Email(input.Username);
        Password(input.Password);
        Rules.Require(new[] { "Warehouse", "Sales", "Logistics" }.Contains(input.Role), "Invalid employee role.");
        Rules.Require(!await db.Set<User>().IgnoreQueryFilters().AnyAsync(u => u.Username == email), "Account already registered.", 409);
        var u = new User { Username = email, Role = input.Role, TenantId = current.TenantId };
        u.PasswordHash = Hasher.HashPassword(u, input.Password);
        var role = await db.Set<Role>().SingleAsync(r => r.Name == input.Role);
        await db.Transaction(async () => { db.Add(u); db.Add(new UserRole { TenantId = current.TenantId, UserId = u.Id, RoleId = role.Id }); return true; });
        return new
        {
            u.Id,
            u.Username,
            u.Role
        };
    }
}

using CeTe.Identity.Domain.Model.Aggregates;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
namespace CeTe.Identity.Infrastructure.Authentication;

public static class SessionValidator
{
    public static async Task Validate(TokenValidatedContext context)
    {
        var principal = context.Principal!;
        if (!Guid.TryParse(principal.FindFirst("jti")?.Value, out var sessionId)
            || !Guid.TryParse(principal.FindFirst("tenant_id")?.Value, out var tenantId))
        {
            context.Fail("Invalid session.");
            return;
        }
        var db = context.HttpContext.RequestServices.GetRequiredService<CeTeDb>();
        var session = await db.Set<UserSession>().IgnoreQueryFilters()
            .SingleOrDefaultAsync(item => item.Id == sessionId && item.TenantId == tenantId);
        if (session == null || session.Revoked || session.ExpiresAt <= DateTimeOffset.UtcNow)
            context.Fail("Session expired.");
    }
}

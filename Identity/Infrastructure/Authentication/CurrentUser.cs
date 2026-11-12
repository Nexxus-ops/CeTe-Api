using System.Security.Claims;
using CeTe.Shared.Application.Services;
namespace CeTe.Identity.Infrastructure.Authentication;

public sealed class CurrentUser(IHttpContextAccessor accessor) : ICurrentUser
{
    public Guid TenantId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue("tenant_id"), out var id) ? id : Guid.Empty;
    public Guid UserId => Guid.TryParse(accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;
    public string Username => accessor.HttpContext?.User.Identity?.Name ?? "";
}

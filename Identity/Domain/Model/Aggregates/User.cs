using CeTe.Shared.Domain.Model;

namespace CeTe.Identity.Domain.Model.Aggregates;

public sealed class User : TenantEntity
{
    public string Username { get; set; } = ""; public string PasswordHash { get; set; } = ""; public string Role { get; set; } = "Admin"; public bool Active { get; set; } = true;
}

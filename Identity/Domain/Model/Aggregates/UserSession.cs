using CeTe.Shared.Domain.Model;

namespace CeTe.Identity.Domain.Model.Aggregates;

public sealed class UserSession : TenantEntity
{
    public Guid UserId
    {
        get; set;
    }
    public DateTimeOffset ExpiresAt
    {
        get; set;
    }
    public bool Revoked
    {
        get; set;
    }
}

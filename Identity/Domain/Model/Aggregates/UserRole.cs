using CeTe.Shared.Domain.Model;

namespace CeTe.Identity.Domain.Model.Aggregates;

public sealed class UserRole : TenantEntity
{
    public Guid UserId
    {
        get; set;
    }
    public Guid RoleId
    {
        get; set;
    }
}

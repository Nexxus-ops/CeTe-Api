namespace CeTe.Shared.Domain.Model;

public abstract class TenantEntity : Entity
{
    public Guid TenantId
    {
        get; set;
    }
}

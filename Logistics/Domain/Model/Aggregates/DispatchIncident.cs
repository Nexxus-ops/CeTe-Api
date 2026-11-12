using CeTe.Shared.Domain.Model;

namespace CeTe.Logistics.Domain.Model.Aggregates;

public sealed class DispatchIncident : TenantEntity
{
    public Guid DispatchId
    {
        get; set;
    }
    public string Description { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

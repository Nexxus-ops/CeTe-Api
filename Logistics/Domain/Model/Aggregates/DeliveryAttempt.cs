using CeTe.Shared.Domain.Model;

namespace CeTe.Logistics.Domain.Model.Aggregates;

public sealed class DeliveryAttempt : TenantEntity
{
    public Guid DispatchStopId
    {
        get; set;
    }
    public string Outcome { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

using CeTe.Shared.Domain.Model;

namespace CeTe.Procurement.Domain.Model.Aggregates;

public sealed class GoodsReceipt : TenantEntity
{
    public Guid PurchaseOrderId
    {
        get; set;
    }
    public Guid UserId
    {
        get; set;
    }
    public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

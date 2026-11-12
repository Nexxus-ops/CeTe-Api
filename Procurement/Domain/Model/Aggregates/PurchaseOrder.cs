using CeTe.Shared.Domain.Model;

namespace CeTe.Procurement.Domain.Model.Aggregates;

public sealed class PurchaseOrder : TenantEntity
{
    public Guid SupplierId
    {
        get; set;
    }
    public Guid WarehouseId
    {
        get; set;
    }
    public string Status { get; set; } = "ORDERED"; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

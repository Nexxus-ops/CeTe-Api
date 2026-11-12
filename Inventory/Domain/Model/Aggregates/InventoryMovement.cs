using CeTe.Shared.Domain.Model;

namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class InventoryMovement : TenantEntity
{
    public Guid ProductId
    {
        get; set;
    }
    public Guid WarehouseId
    {
        get; set;
    }
    public Guid UserId
    {
        get; set;
    }
    public decimal Quantity
    {
        get; set;
    }
    public string Reason { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

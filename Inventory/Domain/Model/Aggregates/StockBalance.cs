using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;

namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class StockBalance : TenantEntity
{
    public Guid ProductId
    {
        get; set;
    }
    public Guid WarehouseId
    {
        get; set;
    }
    public decimal Quantity
    {
        get; private set;
    }
    public void Receive(decimal amount)
    {
        Rules.Require(amount > 0, "Quantity must be positive.");
        Quantity += amount;
    }
    public void Remove(decimal amount)
    {
        Rules.Require(amount > 0, "Quantity must be positive.");
        Rules.Require(Quantity >= amount, "Insufficient stock.", 409);
        Quantity -= amount;
    }
}

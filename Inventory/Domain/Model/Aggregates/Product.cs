using CeTe.Inventory.Domain.Model.ValueObjects;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class Product : TenantEntity
{
    public string Sku { get; private set; } = "";
    public string Name { get; private set; } = "";
    public decimal UnitPrice
    {
        get; private set;
    }
    public decimal MinimumStock { get; private set; } = 10;
    public bool Active { get; private set; } = true;

    public void UpdateDetails(Sku sku, string name, Money unitPrice, decimal minimumStock)
    {
        Rules.Require(!string.IsNullOrWhiteSpace(name), "SKU and name are required.");
        Rules.Require(minimumStock >= 0, "Invalid quantities or price.");
        Sku = sku.Value;
        Name = name.Trim();
        UnitPrice = unitPrice.Amount;
        MinimumStock = minimumStock;
    }

    public void Archive(bool hasStock)
    {
        Rules.Require(!hasStock, "Record an exit before archiving stock.", 409);
        Active = false;
    }
}

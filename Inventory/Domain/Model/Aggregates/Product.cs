using CeTe.Shared.Domain.Model;

namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class Product : TenantEntity
{
    public string Sku { get; set; } = ""; public string Name { get; set; } = ""; public decimal UnitPrice
    {
        get; set;
    }
    public decimal MinimumStock { get; set; } = 10; public bool Active { get; set; } = true;
}

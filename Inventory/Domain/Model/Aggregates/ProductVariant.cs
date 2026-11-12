using CeTe.Shared.Domain.Model;

namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class ProductVariant : TenantEntity
{
    public Guid ProductId
    {
        get; set;
    }
    public string Sku { get; set; } = ""; public string AttributesJson { get; set; } = "{}";
}

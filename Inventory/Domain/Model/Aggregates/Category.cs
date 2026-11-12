using CeTe.Shared.Domain.Model;

namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class Category : TenantEntity
{
    public string Name { get; set; } = "";
}

using CeTe.Shared.Domain.Model;

namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class Warehouse : TenantEntity
{
    public string Name { get; set; } = ""; public string Address { get; set; } = "";
}

using CeTe.Shared.Domain.Model;

namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class Location : TenantEntity
{
    public Guid WarehouseId
    {
        get; set;
    }
    public string Code { get; set; } = "";
}

using CeTe.Shared.Domain.Model;

namespace CeTe.Inventory.Domain.Model.Aggregates;

public sealed class StockLot : TenantEntity
{
    public Guid ProductId
    {
        get; set;
    }
    public string Code { get; set; } = ""; public DateTimeOffset? ExpiresAt
    {
        get; set;
    }
}

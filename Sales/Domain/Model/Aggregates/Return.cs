using CeTe.Shared.Domain.Model;

namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class Return : TenantEntity
{
    public Guid SaleId
    {
        get; set;
    }
    public string Reason { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
}

using CeTe.Shared.Domain.Model;

namespace CeTe.Logistics.Domain.Model.Aggregates;

public sealed class DispatchSale : TenantEntity
{
    public Guid DispatchId
    {
        get; set;
    }
    public Guid SaleId
    {
        get; set;
    }
}

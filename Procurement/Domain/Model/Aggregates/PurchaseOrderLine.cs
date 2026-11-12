using CeTe.Shared.Domain.Model;

namespace CeTe.Procurement.Domain.Model.Aggregates;

public sealed class PurchaseOrderLine : TenantEntity
{
    public Guid PurchaseOrderId
    {
        get; set;
    }
    public Guid ProductId
    {
        get; set;
    }
    public decimal Quantity
    {
        get; set;
    }
    public decimal UnitCost
    {
        get; set;
    }
    public decimal ReceivedQuantity
    {
        get; set;
    }
}

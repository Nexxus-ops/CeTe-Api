using CeTe.Shared.Domain.Model;

namespace CeTe.Procurement.Domain.Model.Aggregates;

public sealed class GoodsReceiptLine : TenantEntity
{
    public Guid GoodsReceiptId
    {
        get; set;
    }
    public Guid PurchaseOrderLineId
    {
        get; set;
    }
    public decimal Quantity
    {
        get; set;
    }
}

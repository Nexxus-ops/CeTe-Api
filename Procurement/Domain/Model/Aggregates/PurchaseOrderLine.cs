using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Procurement.Domain.Model.Aggregates;

public sealed class PurchaseOrderLine : TenantEntity
{
    public Guid PurchaseOrderId
    {
        get; private set;
    }
    public Guid ProductId
    {
        get; private set;
    }
    public decimal Quantity
    {
        get; private set;
    }
    public decimal UnitCost
    {
        get; private set;
    }
    public decimal ReceivedQuantity
    {
        get; private set;
    }

    private PurchaseOrderLine()
    {
    }

    public PurchaseOrderLine(Guid orderId, Guid productId, decimal quantity, Money unitCost)
    {
        Rules.Require(quantity > 0, "Invalid purchase.");
        PurchaseOrderId = orderId;
        ProductId = productId;
        Quantity = quantity;
        UnitCost = unitCost.Amount;
    }

    public void Receive(decimal quantity)
    {
        Rules.Require(quantity > 0 && quantity <= Quantity - ReceivedQuantity, "Quantity exceeds the pending receipt.");
        ReceivedQuantity += quantity;
    }
}

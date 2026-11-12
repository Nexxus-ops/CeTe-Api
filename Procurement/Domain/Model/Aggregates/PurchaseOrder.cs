using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Procurement.Domain.Model.Aggregates;

public sealed class PurchaseOrder : TenantEntity
{
    public Guid SupplierId
    {
        get; private set;
    }
    public Guid WarehouseId
    {
        get; private set;
    }
    public string Status { get; private set; } = "ORDERED";
    public DateTimeOffset CreatedAt { get; private set; } = DateTimeOffset.UtcNow;

    private PurchaseOrder()
    {
    }

    public PurchaseOrder(Guid supplierId, Guid warehouseId)
    {
        SupplierId = supplierId;
        WarehouseId = warehouseId;
    }

    public void Receive(PurchaseOrderLine line, decimal quantity)
    {
        Rules.Require(Status != "RECEIVED", "Purchase already received.", 409);
        Rules.Require(line.PurchaseOrderId == Id, "Resource not found.", 404);
        line.Receive(quantity);
        Status = line.ReceivedQuantity == line.Quantity ? "RECEIVED" : "PARTIAL";
    }
}

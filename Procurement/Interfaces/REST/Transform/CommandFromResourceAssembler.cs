using CeTe.Procurement.Domain.Model.Commands;
using CeTe.Procurement.Interfaces.REST.Resources;
namespace CeTe.Procurement.Interfaces.REST.Transform;

public static class CommandFromResourceAssembler
{
    public static CreatePurchaseCommand ToCommand(this CreatePurchaseResource resource) =>
        new(resource.SupplierName, resource.ProductId, resource.Quantity, resource.UnitCost, resource.WarehouseId);

    public static ReceivePurchaseCommand ToCommand(this ReceivePurchaseResource resource) =>
        new(resource.Quantity);
}

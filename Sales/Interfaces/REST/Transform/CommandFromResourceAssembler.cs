using CeTe.Sales.Domain.Model.Commands;
using CeTe.Sales.Interfaces.REST.Resources;
namespace CeTe.Sales.Interfaces.REST.Transform;

public static class CommandFromResourceAssembler
{
    public static ConfirmSaleCommand ToCommand(this ConfirmSaleResource resource) =>
        new(resource.Id, resource.CustomerName, resource.Items.Select(item => new SaleItemCommand(item.InventoryItemId, item.Quantity)).ToList(), resource.PaymentMethod, resource.WarehouseId);
}

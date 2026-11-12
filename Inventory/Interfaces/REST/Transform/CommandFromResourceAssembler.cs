using CeTe.Inventory.Domain.Model.Commands;
using CeTe.Inventory.Interfaces.REST.Resources;
namespace CeTe.Inventory.Interfaces.REST.Transform;

public static class CommandFromResourceAssembler
{
    public static SaveProductCommand ToCommand(this SaveProductResource resource) =>
        new(resource.Sku, resource.Name, resource.Quantity, resource.UnitPrice, resource.WarehouseId, resource.MinimumStock);

    public static MoveStockCommand ToCommand(this MoveStockResource resource) =>
        new(resource.Quantity, resource.Reason, resource.WarehouseId);
}

namespace CeTe.Inventory.Interfaces.REST.Resources;

public record MoveStockResource(decimal Quantity, string Reason, Guid? WarehouseId = null);

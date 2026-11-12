namespace CeTe.Inventory.Domain.Model.Commands;

public record MoveStockCommand(decimal Quantity, string Reason, Guid? WarehouseId = null);

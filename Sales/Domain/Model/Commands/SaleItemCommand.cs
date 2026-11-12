namespace CeTe.Sales.Domain.Model.Commands;

public record SaleItemCommand(Guid InventoryItemId, decimal Quantity);

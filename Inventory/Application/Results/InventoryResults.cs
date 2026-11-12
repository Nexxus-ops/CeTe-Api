namespace CeTe.Inventory.Application.Results;

public record InventoryItemResult(Guid Id, string Sku, string Name, decimal UnitPrice, decimal MinimumStock, decimal Quantity);
public record StockMovementResult(Guid ProductId, decimal Quantity);
public record ArchivedProductResult(Guid Id);

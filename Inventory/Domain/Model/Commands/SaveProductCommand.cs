namespace CeTe.Inventory.Domain.Model.Commands;

public record SaveProductCommand(string Sku, string Name, decimal Quantity, decimal UnitPrice, Guid? WarehouseId = null, decimal MinimumStock = 10);

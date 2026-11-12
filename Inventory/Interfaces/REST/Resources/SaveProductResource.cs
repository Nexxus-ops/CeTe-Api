namespace CeTe.Inventory.Interfaces.REST.Resources;

public record SaveProductResource(string Sku, string Name, decimal Quantity, decimal UnitPrice, Guid? WarehouseId = null, decimal MinimumStock = 10);

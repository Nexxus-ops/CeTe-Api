namespace CeTe.Procurement.Interfaces.REST.Resources;

public record CreatePurchaseResource(string SupplierName, Guid ProductId, decimal Quantity, decimal UnitCost, Guid? WarehouseId = null);

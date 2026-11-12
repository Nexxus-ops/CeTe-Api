namespace CeTe.Procurement.Domain.Model.Commands;

public record CreatePurchaseCommand(string SupplierName, Guid ProductId, decimal Quantity, decimal UnitCost, Guid? WarehouseId = null);

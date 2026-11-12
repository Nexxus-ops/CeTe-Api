namespace CeTe.Sales.Interfaces.REST.Resources;

public record ConfirmSaleResource(Guid Id, string CustomerName, List<SaleItemResource> Items, string PaymentMethod = "CASH", Guid? WarehouseId = null);

namespace CeTe.Sales.Domain.Model.Commands;

public record ConfirmSaleCommand(Guid Id, string CustomerName, List<SaleItemCommand> Items, string PaymentMethod = "CASH", Guid? WarehouseId = null);

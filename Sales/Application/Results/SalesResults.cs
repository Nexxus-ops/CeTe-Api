namespace CeTe.Sales.Application.Results;

public record SaleItemResult(Guid InventoryItemId, string Sku, string Name, decimal Quantity, decimal UnitPrice);
public record SaleResult(Guid Id, string CustomerName, decimal TotalAmount, string Status,
    DateTimeOffset CreatedAt, Guid WarehouseId, IEnumerable<SaleItemResult> Items);
public record ReceiptItemResult(string Sku, string Name, decimal Quantity, decimal UnitPrice, decimal Amount);
public record ReceiptResult(string Number, Guid OrderId, string CompanyName, string Ruc, string Address,
    string Cashier, DateTimeOffset IssuedAt, string TimeZone, string CustomerName, string Status,
    string Currency, decimal Subtotal, decimal Total, string PaymentMethod, string DocumentType,
    IEnumerable<ReceiptItemResult> Items);

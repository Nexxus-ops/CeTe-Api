using System.Linq.Expressions;
using CeTe.Domain;

namespace CeTe.Application;
public interface ICeTeRepository
{
    Task<List<T>> List<T>(Expression<Func<T, bool>>? predicate = null) where T : Entity;
    Task<T?> Find<T>(Guid id) where T : Entity;
    void Add<T>(T item) where T : Entity;
    Task Save();
    Task<T> Transaction<T>(Func<Task<T>> action);
}
public interface ICurrentUser { Guid TenantId { get; } Guid UserId { get; } string Username { get; } }
public record InventoryInput(string Sku, string Name, decimal Quantity, decimal UnitPrice, Guid? WarehouseId = null, decimal MinimumStock = 10);
public record SaleItemInput(Guid InventoryItemId, decimal Quantity);
public record SaleInput(Guid Id, string CustomerName, List<SaleItemInput> Items, string PaymentMethod = "CASH", Guid? WarehouseId = null);
public record DispatchInput(string DriverName, string LicensePlate, List<Guid> SaleIds);
public record MovementInput(decimal Quantity, string Reason, Guid? WarehouseId = null);
public record PurchaseInput(string SupplierName, Guid ProductId, decimal Quantity, decimal UnitCost, Guid? WarehouseId = null);
public record ReceiveInput(decimal Quantity);
public record StatusInput(string Status);
public record IncidentInput(string Description);

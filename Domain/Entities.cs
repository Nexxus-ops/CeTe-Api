namespace CeTe.Domain;

public abstract class Entity { public Guid Id { get; set; } = Guid.NewGuid(); }
public abstract class TenantEntity : Entity { public Guid TenantId { get; set; } }
public sealed class BusinessException(string message, int status = 422) : Exception(message) { public int Status { get; } = status; }
public static class Rules
{
    public static void Require(bool condition, string message, int status = 422) { if (!condition) throw new BusinessException(message, status); }
    public static decimal Money(decimal amount) => decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
}
public sealed class Tenant : Entity { public string Name { get; set; } = ""; public string Ruc { get; set; } = ""; public string Address { get; set; } = ""; }
public sealed class SubscriptionPlan : Entity { public string Name { get; set; } = ""; public decimal MonthlyPrice { get; set; } }
public sealed class TenantSubscription : TenantEntity { public Guid SubscriptionPlanId { get; set; } public string Status { get; set; } = "TRIAL"; }
public sealed class User : TenantEntity { public string Username { get; set; } = ""; public string PasswordHash { get; set; } = ""; public string Role { get; set; } = "Admin"; public bool Active { get; set; } = true; }
public sealed class Role : TenantEntity { public string Name { get; set; } = ""; }
public sealed class UserRole : TenantEntity { public Guid UserId { get; set; } public Guid RoleId { get; set; } }
public sealed class UserSession : TenantEntity { public Guid UserId { get; set; } public DateTimeOffset ExpiresAt { get; set; } public bool Revoked { get; set; } }
public sealed class Warehouse : TenantEntity { public string Name { get; set; } = ""; public string Address { get; set; } = ""; }
public sealed class Location : TenantEntity { public Guid WarehouseId { get; set; } public string Code { get; set; } = ""; }
public sealed class Category : TenantEntity { public string Name { get; set; } = ""; }
public sealed class Product : TenantEntity { public string Sku { get; set; } = ""; public string Name { get; set; } = ""; public decimal UnitPrice { get; set; } public decimal MinimumStock { get; set; } = 10; public bool Active { get; set; } = true; }
public sealed class ProductVariant : TenantEntity { public Guid ProductId { get; set; } public string Sku { get; set; } = ""; public string AttributesJson { get; set; } = "{}"; }
public sealed class StockLot : TenantEntity { public Guid ProductId { get; set; } public string Code { get; set; } = ""; public DateTimeOffset? ExpiresAt { get; set; } }
public sealed class StockBalance : TenantEntity
{
    public Guid ProductId { get; set; } public Guid WarehouseId { get; set; } public decimal Quantity { get; private set; }
    public void Receive(decimal amount) { Rules.Require(amount > 0, "Quantity must be positive."); Quantity += amount; }
    public void Remove(decimal amount) { Rules.Require(amount > 0, "Quantity must be positive."); Rules.Require(Quantity >= amount, "Insufficient stock.", 409); Quantity -= amount; }
}
public sealed class InventoryMovement : TenantEntity { public Guid ProductId { get; set; } public Guid WarehouseId { get; set; } public Guid UserId { get; set; } public decimal Quantity { get; set; } public string Reason { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; }
public sealed class Supplier : TenantEntity { public string Name { get; set; } = ""; public string Contact { get; set; } = ""; }
public sealed class PurchaseOrder : TenantEntity { public Guid SupplierId { get; set; } public Guid WarehouseId { get; set; } public string Status { get; set; } = "ORDERED"; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; }
public sealed class PurchaseOrderLine : TenantEntity { public Guid PurchaseOrderId { get; set; } public Guid ProductId { get; set; } public decimal Quantity { get; set; } public decimal UnitCost { get; set; } public decimal ReceivedQuantity { get; set; } }
public sealed class GoodsReceipt : TenantEntity { public Guid PurchaseOrderId { get; set; } public Guid UserId { get; set; } public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; }
public sealed class GoodsReceiptLine : TenantEntity { public Guid GoodsReceiptId { get; set; } public Guid PurchaseOrderLineId { get; set; } public decimal Quantity { get; set; } }
public sealed class Customer : TenantEntity { public string Name { get; set; } = ""; public string Contact { get; set; } = ""; }
public sealed class Sale : TenantEntity
{
    public string CustomerName { get; set; } = ""; public Guid WarehouseId { get; set; } public Guid UserId { get; set; } public decimal TotalAmount { get; private set; }
    public string Status { get; private set; } = "COMPLETED"; public string RequestHash { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public void Confirm(IEnumerable<SaleLine> lines) { var list = lines.ToList(); Rules.Require(list.Count > 0, "The sale is empty."); Rules.Require(list.All(l => l.Quantity > 0 && l.UnitPrice >= 0), "Invalid sale lines."); TotalAmount = Rules.Money(list.Sum(l => l.Quantity * l.UnitPrice)); }
    public void Cancel() { Rules.Require(Status == "COMPLETED", "Sale already cancelled.", 409); Status = "CANCELLED"; }
}
public sealed class SaleLine : TenantEntity { public Guid SaleId { get; set; } public Guid ProductId { get; set; } public string Sku { get; set; } = ""; public string Name { get; set; } = ""; public decimal Quantity { get; set; } public decimal UnitPrice { get; set; } }
public sealed class Payment : TenantEntity { public Guid SaleId { get; set; } public string Method { get; set; } = "CASH"; public decimal Amount { get; set; } public string Status { get; set; } = "PAID"; }
public sealed class SalesReceipt : TenantEntity { public Guid SaleId { get; set; } public string Number { get; set; } = ""; public string CompanyName { get; set; } = ""; public string Ruc { get; set; } = ""; public string Address { get; set; } = ""; public string Cashier { get; set; } = ""; public DateTimeOffset IssuedAt { get; set; } = DateTimeOffset.UtcNow; }
public sealed class Driver : TenantEntity { public string Name { get; set; } = ""; public string License { get; set; } = ""; }
public sealed class Vehicle : TenantEntity { public string LicensePlate { get; set; } = ""; public decimal Capacity { get; set; } }
public sealed class Dispatch : TenantEntity
{
    public string DriverName { get; set; } = ""; public string LicensePlate { get; set; } = ""; public string Status { get; private set; } = "PENDING"; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;
    public void Transition(string next) { Rules.Require((Status, next) is ("PENDING", "IN_TRANSIT") or ("IN_TRANSIT", "DELIVERED") or ("IN_TRANSIT", "INCIDENT") or ("INCIDENT", "IN_TRANSIT"), "Invalid dispatch transition.", 409); Status = next; }
}
public sealed class DispatchSale : TenantEntity { public Guid DispatchId { get; set; } public Guid SaleId { get; set; } }
public sealed class DispatchStop : TenantEntity { public Guid DispatchId { get; set; } public string Address { get; set; } = ""; public int Sequence { get; set; } }
public sealed class DeliveryAttempt : TenantEntity { public Guid DispatchStopId { get; set; } public string Outcome { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; }
public sealed class DispatchIncident : TenantEntity { public Guid DispatchId { get; set; } public string Description { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; }
public sealed class Return : TenantEntity { public Guid SaleId { get; set; } public string Reason { get; set; } = ""; public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow; }
public sealed class ReturnLine : TenantEntity { public Guid ReturnId { get; set; } public Guid SaleLineId { get; set; } public decimal Quantity { get; set; } public string Resolution { get; set; } = ""; }
public sealed class Notification : TenantEntity { public string Message { get; set; } = ""; public string EventKey { get; set; } = ""; public bool IsRead { get; set; } }

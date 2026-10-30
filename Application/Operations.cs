using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CeTe.Domain;

namespace CeTe.Application;

public sealed class Operations(ICeTeRepository repository, ICurrentUser user)
{
    private async Task<T> Required<T>(Guid id) where T : Entity => await repository.Find<T>(id) ?? throw new BusinessException("Resource not found.", 404);
    private async Task<Guid> WarehouseId(Guid? id)
    {
        var warehouse = id.HasValue ? await repository.Find<Warehouse>(id.Value) : (await repository.List<Warehouse>()).FirstOrDefault();
        return warehouse?.Id ?? throw new BusinessException("Warehouse not found.", 404);
    }
    private async Task<StockBalance> Balance(Guid productId, Guid warehouseId)
    {
        var b = (await repository.List<StockBalance>(x => x.ProductId == productId && x.WarehouseId == warehouseId)).SingleOrDefault();
        if (b != null)
            return b;
        b = new StockBalance { ProductId = productId, WarehouseId = warehouseId };
        repository.Add(b);
        return b;
    }
    private void Movement(Guid productId, Guid warehouseId, decimal quantity, string reason) => repository.Add(new InventoryMovement { ProductId = productId, WarehouseId = warehouseId, UserId = user.UserId, Quantity = quantity, Reason = reason });
    public async Task<object[]> Inventory()
    {
        var balances = await repository.List<StockBalance>();
        return (await repository.List<Product>(x => x.Active)).Select(p => (object)new { p.Id, p.Sku, p.Name, p.UnitPrice, p.MinimumStock, quantity = balances.Where(b => b.ProductId == p.Id).Sum(b => b.Quantity) }).ToArray();
    }
    public Task<object> SaveProduct(Guid? id, InventoryInput input) => repository.Transaction<object>(async () =>
    {
        Rules.Require(!string.IsNullOrWhiteSpace(input.Sku) && !string.IsNullOrWhiteSpace(input.Name), "SKU and name are required.");
        Rules.Require(input.Quantity >= 0 && input.UnitPrice >= 0 && input.MinimumStock >= 0, "Invalid quantities or price.");
        var sku = input.Sku.Trim().ToUpperInvariant();
        Rules.Require(!(await repository.List<Product>(p => p.Sku == sku && p.Id != id)).Any(), "SKU already exists.", 409);
        var p = id.HasValue ? await Required<Product>(id.Value) : new Product();
        if (!id.HasValue)
            repository.Add(p);
        p.Sku = sku;
        p.Name = input.Name.Trim();
        p.UnitPrice = Rules.Money(input.UnitPrice);
        p.MinimumStock = input.MinimumStock;
        // Initial stock is an auditable opening movement. Later adjustments use movement endpoints.
        if (!id.HasValue && input.Quantity > 0)
        {
            var w = await WarehouseId(input.WarehouseId);
            (await Balance(p.Id, w)).Receive(input.Quantity);
            Movement(p.Id, w, input.Quantity, "OPENING");
        }
        await repository.Save();
        return new
        {
            p.Id,
            p.Sku,
            p.Name,
            p.UnitPrice,
            p.MinimumStock,
            quantity = (await repository.List<StockBalance>(b => b.ProductId == p.Id)).Sum(b => b.Quantity)
        };
    });
    public Task<object> Move(Guid id, MovementInput input, bool receive) => repository.Transaction<object>(async () =>
    {
        await Required<Product>(id);
        Rules.Require(!string.IsNullOrWhiteSpace(input.Reason), "A movement reason is required.");
        var w = await WarehouseId(input.WarehouseId);
        var balance = await Balance(id, w);
        if (receive)
            balance.Receive(input.Quantity);
        else
            balance.Remove(input.Quantity);
        Movement(id, w, receive ? input.Quantity : -input.Quantity, input.Reason.Trim());
        return new
        {
            productId = id,
            balance.Quantity
        };
    });
    public Task<object> Archive(Guid id) => repository.Transaction<object>(async () => { var p = await Required<Product>(id); Rules.Require(!(await repository.List<StockBalance>(b => b.ProductId == id)).Any(b => b.Quantity != 0), "Record an exit before archiving stock.", 409); p.Active = false; return new { p.Id }; });
    public async Task<object> History(Guid id)
    {
        await Required<Product>(id);
        return (await repository.List<InventoryMovement>(m => m.ProductId == id)).OrderByDescending(m => m.CreatedAt);
    }
    public async Task<object> SaleView(Sale sale)
    {
        var items = await repository.List<SaleLine>(l => l.SaleId == sale.Id);
        return new
        {
            sale.Id,
            sale.CustomerName,
            sale.TotalAmount,
            sale.Status,
            sale.CreatedAt,
            sale.WarehouseId,
            items = items.Select(l => new { inventoryItemId = l.ProductId, l.Sku, l.Name, l.Quantity, l.UnitPrice })
        };
    }
    public async Task<object[]> Sales()
    {
        var list = (await repository.List<Sale>()).OrderByDescending(s => s.CreatedAt);
        var result = new List<object>();
        foreach (var s in list)
            result.Add(await SaleView(s));
        return result.ToArray();
    }
    public Task<object> Checkout(SaleInput input) => repository.Transaction<object>(async () =>
    {
        Rules.Require(input.Id != Guid.Empty, "An idempotency identifier is required.");
        Rules.Require(input.Items.Count > 0 && input.Items.Select(i => i.InventoryItemId).Distinct().Count() == input.Items.Count, "Use one line per product.");
        Rules.Require(new[] { "CASH", "CARD", "TRANSFER" }.Contains(input.PaymentMethod), "Unsupported payment method.");
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
        var existing = await repository.Find<Sale>(input.Id);
        if (existing != null)
        {
            Rules.Require(existing.RequestHash == hash, "This identifier belongs to a different request.", 409);
            return await SaleView(existing);
        }
        var w = await WarehouseId(input.WarehouseId);
        var sale = new Sale { Id = input.Id, CustomerName = string.IsNullOrWhiteSpace(input.CustomerName) ? "General customer" : input.CustomerName.Trim(), WarehouseId = w, UserId = user.UserId, RequestHash = hash };
        var lines = new List<SaleLine>();
        foreach (var item in input.Items)
        {
            var p = await Required<Product>(item.InventoryItemId);
            Rules.Require(p.Active, "The product is archived.");
            (await Balance(p.Id, w)).Remove(item.Quantity);
            var line = new SaleLine { SaleId = sale.Id, ProductId = p.Id, Sku = p.Sku, Name = p.Name, Quantity = item.Quantity, UnitPrice = p.UnitPrice };
            lines.Add(line);
            repository.Add(line);
            Movement(p.Id, w, -item.Quantity, $"SALE:{sale.Id}");
        }
        sale.Confirm(lines);
        repository.Add(sale);
        repository.Add(new Payment { SaleId = sale.Id, Amount = sale.TotalAmount, Method = input.PaymentMethod });
        var tenant = await Required<Tenant>(user.TenantId);
        var warehouse = await Required<Warehouse>(w);
        repository.Add(new SalesReceipt { SaleId = sale.Id, Number = $"DEMO-{sale.Id.ToString("N")[..12].ToUpperInvariant()}", CompanyName = tenant.Name, Ruc = tenant.Ruc, Address = warehouse.Address, Cashier = user.Username, IssuedAt = sale.CreatedAt });
        await repository.Save();
        return await SaleView(sale);
    });
    public Task<object> CancelSale(Guid id) => repository.Transaction<object>(async () =>
    {
        var sale = await Required<Sale>(id);
        Rules.Require(!(await repository.List<DispatchSale>(d => d.SaleId == id)).Any(), "A dispatched sale cannot be cancelled. Register its return instead.", 409);
        sale.Cancel();
        foreach (var line in await repository.List<SaleLine>(l => l.SaleId == id))
        {
            (await Balance(line.ProductId, sale.WarehouseId)).Receive(line.Quantity);
            Movement(line.ProductId, sale.WarehouseId, line.Quantity, $"CANCEL:{id}");
        }
        foreach (var p in await repository.List<Payment>(p => p.SaleId == id))
            p.Status = "REVERSED";
        return await SaleView(sale);
    });
    public async Task<object> Receipt(Guid id)
    {
        var sale = await Required<Sale>(id);
        var receipt = (await repository.List<SalesReceipt>(r => r.SaleId == id)).SingleOrDefault() ?? throw new BusinessException("Receipt not found.", 404);
        var lines = await repository.List<SaleLine>(l => l.SaleId == id);
        var payment = (await repository.List<Payment>(p => p.SaleId == id)).Single();
        return new
        {
            receipt.Number,
            orderId = sale.Id,
            receipt.CompanyName,
            receipt.Ruc,
            receipt.Address,
            receipt.Cashier,
            receipt.IssuedAt,
            timeZone = "America/Lima",
            sale.CustomerName,
            sale.Status,
            currency = "PEN",
            subtotal = sale.TotalAmount,
            total = sale.TotalAmount,
            paymentMethod = payment.Method,
            documentType = "DEMONSTRATION_RECEIPT",
            items = lines.Select(l => new { l.Sku, l.Name, l.Quantity, l.UnitPrice, amount = Rules.Money(l.Quantity * l.UnitPrice) })
        };
    }
    public async Task<object[]> Dispatches()
    {
        var relations = await repository.List<DispatchSale>();
        return (await repository.List<Dispatch>()).OrderByDescending(d => d.CreatedAt).Select(d => (object)new { d.Id, d.DriverName, d.LicensePlate, d.Status, d.CreatedAt, saleIds = relations.Where(r => r.DispatchId == d.Id).Select(r => r.SaleId) }).ToArray();
    }
    public Task<object> CreateDispatch(DispatchInput input) => repository.Transaction<object>(async () =>
    {
        Rules.Require(!string.IsNullOrWhiteSpace(input.DriverName) && !string.IsNullOrWhiteSpace(input.LicensePlate) && input.SaleIds.Count > 0, "Driver, plate and sales are required.");
        Rules.Require(input.SaleIds.Distinct().Count() == input.SaleIds.Count, "Duplicate sale.");
        var plate = input.LicensePlate.Trim().ToUpperInvariant();
        Rules.Require(!(await repository.List<Dispatch>(d => d.Status != "DELIVERED")).Any(d => d.LicensePlate == plate || d.DriverName == input.DriverName.Trim()), "Driver or vehicle already assigned.", 409);
        var dispatch = new Dispatch { DriverName = input.DriverName.Trim(), LicensePlate = plate };
        repository.Add(dispatch);
        foreach (var id in input.SaleIds)
        {
            var sale = await Required<Sale>(id);
            Rules.Require(sale.Status == "COMPLETED", "Only completed sales can be dispatched.");
            Rules.Require(!(await repository.List<DispatchSale>(d => d.SaleId == id)).Any(), "Sale already assigned.", 409);
            repository.Add(new DispatchSale { DispatchId = dispatch.Id, SaleId = id });
        }
        await repository.Save();
        return new
        {
            dispatch.Id,
            dispatch.DriverName,
            dispatch.LicensePlate,
            dispatch.Status,
            dispatch.CreatedAt,
            input.SaleIds
        };
    });
    public Task<object> SetDispatchStatus(Guid id, string next) => repository.Transaction<object>(async () => { var d = await Required<Dispatch>(id); d.Transition(next); return new { d.Id, d.DriverName, d.LicensePlate, d.Status, d.CreatedAt }; });
    public Task<object> Incident(Guid id, string description) => repository.Transaction<object>(async () => { var d = await Required<Dispatch>(id); Rules.Require(!string.IsNullOrWhiteSpace(description), "Description is required."); d.Transition("INCIDENT"); var incident = new DispatchIncident { DispatchId = id, Description = description.Trim() }; repository.Add(incident); return incident; });
    public Task<object> Purchase(PurchaseInput input) => repository.Transaction<object>(async () =>
    {
        await Required<Product>(input.ProductId);
        Rules.Require(input.Quantity > 0 && input.UnitCost >= 0 && !string.IsNullOrWhiteSpace(input.SupplierName), "Invalid purchase.");
        var supplier = (await repository.List<Supplier>(s => s.Name == input.SupplierName.Trim())).FirstOrDefault();
        if (supplier == null)
        {
            supplier = new Supplier { Name = input.SupplierName.Trim() };
            repository.Add(supplier);
        }
        var order = new PurchaseOrder { SupplierId = supplier.Id, WarehouseId = await WarehouseId(input.WarehouseId) };
        repository.Add(order);
        repository.Add(new PurchaseOrderLine { PurchaseOrderId = order.Id, ProductId = input.ProductId, Quantity = input.Quantity, UnitCost = Rules.Money(input.UnitCost) });
        return order;
    });
    public async Task<object> Purchases()
    {
        var lines = await repository.List<PurchaseOrderLine>();
        var suppliers = await repository.List<Supplier>();
        return (await repository.List<PurchaseOrder>()).OrderByDescending(o => o.CreatedAt).Select(o => new { o.Id, o.Status, o.CreatedAt, supplierName = suppliers.Single(s => s.Id == o.SupplierId).Name, lines = lines.Where(l => l.PurchaseOrderId == o.Id) });
    }
    public Task<object> Receive(Guid id, decimal quantity) => repository.Transaction<object>(async () =>
    {
        var order = await Required<PurchaseOrder>(id);
        Rules.Require(order.Status != "RECEIVED", "Purchase already received.", 409);
        var line = (await repository.List<PurchaseOrderLine>(l => l.PurchaseOrderId == id)).Single();
        Rules.Require(quantity > 0 && quantity <= line.Quantity - line.ReceivedQuantity, "Quantity exceeds the pending receipt.");
        var receipt = new GoodsReceipt { PurchaseOrderId = id, UserId = user.UserId };
        repository.Add(receipt);
        repository.Add(new GoodsReceiptLine { GoodsReceiptId = receipt.Id, PurchaseOrderLineId = line.Id, Quantity = quantity });
        (await Balance(line.ProductId, order.WarehouseId)).Receive(quantity);
        Movement(line.ProductId, order.WarehouseId, quantity, $"RECEIPT:{receipt.Id}");
        line.ReceivedQuantity += quantity;
        order.Status = line.ReceivedQuantity == line.Quantity ? "RECEIVED" : "PARTIAL";
        return order;
    });
    public async Task<object> Dashboard()
    {
        var sales = (await repository.List<Sale>(s => s.Status == "COMPLETED"));
        var balances = await repository.List<StockBalance>();
        var products = await repository.List<Product>(p => p.Active);
        var today = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-5)).Date;
        var daily = sales.Where(s => s.CreatedAt.ToOffset(TimeSpan.FromHours(-5)).Date == today).ToList();
        return new
        {
            dailyRevenue = daily.Sum(s => s.TotalAmount),
            dailySales = daily.Count,
            productCount = products.Count,
            lowStock = products.Where(p => balances.Where(b => b.ProductId == p.Id).Sum(b => b.Quantity) <= p.MinimumStock).Select(p => new { p.Id, p.Name, p.MinimumStock, quantity = balances.Where(b => b.ProductId == p.Id).Sum(b => b.Quantity) }),
            trends = sales.GroupBy(s => s.CreatedAt.ToOffset(TimeSpan.FromHours(-5)).ToString("yyyy-MM-dd")).OrderBy(g => g.Key).TakeLast(14).Select(g => new { date = g.Key, total = g.Sum(s => s.TotalAmount) }),
            pendingDispatches = (await repository.List<Dispatch>(d => d.Status != "DELIVERED")).Count
        };
    }
}

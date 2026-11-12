using CeTe.Procurement.Domain.Model.Aggregates;
using CeTe.Procurement.Domain.Repositories;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using CeTe.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace CeTe.Procurement.Infrastructure.Persistence.EFC.Repositories;

public sealed class ProcurementRepository(CeTeDb db) : BaseRepository(db), IProcurementRepository
{
    public Task<List<Supplier>> Suppliers() => List<Supplier>();
    public Task<Supplier?> FindSupplier(string name) => SupplierByName(name);
    public Task<List<PurchaseOrder>> Orders() => List<PurchaseOrder>();
    public Task<PurchaseOrder?> FindOrder(Guid id) => Find<PurchaseOrder>(id);
    public Task<List<PurchaseOrderLine>> Lines() => List<PurchaseOrderLine>();
    public Task<PurchaseOrderLine> OrderLine(Guid id) => SingleLine(id);
    public void AddSupplier(Supplier supplier) => Add(supplier);
    public void AddOrder(PurchaseOrder order) => Add(order);
    public void AddLine(PurchaseOrderLine line) => Add(line);
    public void AddReceipt(GoodsReceipt receipt) => Add(receipt);
    public void AddReceiptLine(GoodsReceiptLine line) => Add(line);

    private async Task<Supplier?> SupplierByName(string name) =>
        (await List<Supplier>(supplier => supplier.Name == name)).FirstOrDefault();
    private async Task<PurchaseOrderLine> SingleLine(Guid id) =>
        (await List<PurchaseOrderLine>(line => line.PurchaseOrderId == id)).Single();
}

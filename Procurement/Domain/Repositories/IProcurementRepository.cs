using CeTe.Procurement.Domain.Model.Aggregates;

namespace CeTe.Procurement.Domain.Repositories;

public interface IProcurementRepository
{
    Task<List<Supplier>> Suppliers();
    Task<Supplier?> FindSupplier(string name);
    Task<List<PurchaseOrder>> Orders();
    Task<PurchaseOrder?> FindOrder(Guid id);
    Task<List<PurchaseOrderLine>> Lines();
    Task<PurchaseOrderLine> OrderLine(Guid id);
    void AddSupplier(Supplier supplier);
    void AddOrder(PurchaseOrder order);
    void AddLine(PurchaseOrderLine line);
    void AddReceipt(GoodsReceipt receipt);
    void AddReceiptLine(GoodsReceiptLine line);
}

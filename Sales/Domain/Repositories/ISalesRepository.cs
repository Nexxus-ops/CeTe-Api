using CeTe.Sales.Domain.Model.Aggregates;

namespace CeTe.Sales.Domain.Repositories;

public interface ISalesRepository
{
    Task<List<Sale>> Sales();
    Task<Sale?> FindSale(Guid id);
    Task<List<SaleLine>> Lines(Guid saleId);
    Task<List<Payment>> Payments(Guid saleId);
    Task<SalesReceipt?> Receipt(Guid saleId);
    void AddSale(Sale sale);
    void AddLine(SaleLine line);
    void AddPayment(Payment payment);
    void AddReceipt(SalesReceipt receipt);
}

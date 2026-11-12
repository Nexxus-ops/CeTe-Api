using CeTe.Sales.Domain.Model.Aggregates;
using CeTe.Sales.Domain.Repositories;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using CeTe.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace CeTe.Sales.Infrastructure.Persistence.EFC.Repositories;

public sealed class SalesRepository(CeTeDb db) : BaseRepository(db), ISalesRepository
{
    public Task<List<Sale>> Sales() => List<Sale>();
    public Task<Sale?> FindSale(Guid id) => Find<Sale>(id);
    public Task<List<SaleLine>> Lines(Guid saleId) => List<SaleLine>(line => line.SaleId == saleId);
    public Task<List<Payment>> Payments(Guid saleId) => List<Payment>(payment => payment.SaleId == saleId);
    public Task<SalesReceipt?> Receipt(Guid saleId) => FindReceipt(saleId);
    public void AddSale(Sale sale) => Add(sale);
    public void AddLine(SaleLine line) => Add(line);
    public void AddPayment(Payment payment) => Add(payment);
    public void AddReceipt(SalesReceipt receipt) => Add(receipt);

    private async Task<SalesReceipt?> FindReceipt(Guid saleId) =>
        (await List<SalesReceipt>(receipt => receipt.SaleId == saleId)).SingleOrDefault();
}

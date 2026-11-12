using CeTe.Sales.Application.Results;
using CeTe.Sales.Application.Services;
using CeTe.Sales.Domain.Model.Queries;
using CeTe.Sales.Domain.Repositories;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.ValueObjects;
namespace CeTe.Sales.Application.Internal.QueryServices;

public sealed class SalesQueryService(ISalesRepository repository) : ISalesQueryService
{
    public async Task<SaleResult> SaleView(GetSaleByIdQuery query)
    {
        var sale = await repository.FindSale(query.SaleId) ?? throw new BusinessException("Resource not found.", 404);
        var lines = await repository.Lines(sale.Id);
        return new SaleResult(sale.Id, sale.CustomerName, sale.TotalAmount, sale.Status, sale.CreatedAt,
            sale.WarehouseId, lines.Select(line =>
                new SaleItemResult(line.ProductId, line.Sku, line.Name, line.Quantity, line.UnitPrice)));
    }

    public async Task<SaleResult[]> Sales(GetSalesQuery query)
    {
        var sales = (await repository.Sales()).OrderByDescending(sale => sale.CreatedAt);
        var results = new List<SaleResult>();
        foreach (var sale in sales)
            results.Add(await SaleView(new GetSaleByIdQuery(sale.Id)));
        return results.ToArray();
    }

    public async Task<ReceiptResult> Receipt(GetReceiptBySaleIdQuery query)
    {
        var sale = await repository.FindSale(query.SaleId) ?? throw new BusinessException("Resource not found.", 404);
        var receipt = await repository.Receipt(query.SaleId) ?? throw new BusinessException("Receipt not found.", 404);
        var lines = await repository.Lines(query.SaleId);
        var payment = (await repository.Payments(query.SaleId)).Single();
        return new ReceiptResult(receipt.Number, sale.Id, receipt.CompanyName, receipt.Ruc, receipt.Address,
            receipt.Cashier, receipt.IssuedAt, "America/Lima", sale.CustomerName, sale.Status, "PEN",
            sale.TotalAmount, sale.TotalAmount, payment.Method, "DEMONSTRATION_RECEIPT",
            lines.Select(line => new ReceiptItemResult(line.Sku, line.Name, line.Quantity, line.UnitPrice,
                Rules.Money(line.Quantity * line.UnitPrice))));
    }

    public async Task<string> Status(GetSaleStatusQuery query) =>
        (await repository.FindSale(query.SaleId) ?? throw new BusinessException("Resource not found.", 404)).Status;
}

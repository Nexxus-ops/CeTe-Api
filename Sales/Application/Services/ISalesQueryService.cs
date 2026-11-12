using CeTe.Sales.Application.Results;
using CeTe.Sales.Domain.Model.Queries;
using CeTe.Sales.Domain.Model.Commands;
using CeTe.Sales.Domain.Model.Aggregates;
namespace CeTe.Sales.Application.Services;

public interface ISalesQueryService
{
    Task<SaleResult> SaleView(GetSaleByIdQuery query);
    Task<SaleResult[]> Sales(GetSalesQuery query);
    Task<ReceiptResult> Receipt(GetReceiptBySaleIdQuery query);
    Task<string> Status(GetSaleStatusQuery query);
}

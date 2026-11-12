using CeTe.Procurement.Application.Results;
using CeTe.Procurement.Application.Services;
using CeTe.Procurement.Domain.Model.Queries;
using CeTe.Procurement.Domain.Repositories;
namespace CeTe.Procurement.Application.Internal.QueryServices;

public sealed class ProcurementQueryService(IProcurementRepository repository) : IProcurementQueryService
{
    public async Task<PurchaseResult[]> Purchases(GetPurchasesQuery query)
    {
        var lines = await repository.Lines();
        var suppliers = await repository.Suppliers();
        return (await repository.Orders()).OrderByDescending(order => order.CreatedAt)
            .Select(order => new PurchaseResult(order.Id, order.Status, order.CreatedAt,
                suppliers.Single(supplier => supplier.Id == order.SupplierId).Name,
                lines.Where(line => line.PurchaseOrderId == order.Id))).ToArray();
    }
}

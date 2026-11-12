using CeTe.Procurement.Domain.Model.Aggregates;
using CeTe.Procurement.Application.Results;
using CeTe.Procurement.Domain.Model.Queries;
using CeTe.Procurement.Domain.Model.Commands;

namespace CeTe.Procurement.Application.Services;

public interface IProcurementQueryService
{
    Task<PurchaseResult[]> Purchases(GetPurchasesQuery query);
}

using CeTe.Logistics.Domain.Model.Aggregates;
using CeTe.Logistics.Application.Results;
using CeTe.Logistics.Domain.Model.Queries;
using CeTe.Logistics.Domain.Model.Commands;

namespace CeTe.Logistics.Application.Services;

public interface ILogisticsQueryService
{
    Task<DispatchResult[]> Dispatches(GetDispatchesQuery query);
}

using CeTe.Logistics.Application.Results;
using CeTe.Logistics.Application.Services;
using CeTe.Logistics.Domain.Model.Queries;
using CeTe.Logistics.Domain.Repositories;
namespace CeTe.Logistics.Application.Internal.QueryServices;

public sealed class LogisticsQueryService(ILogisticsRepository repository) : ILogisticsQueryService
{
    public async Task<DispatchResult[]> Dispatches(GetDispatchesQuery query)
    {
        var assignments = await repository.Assignments();
        return (await repository.Dispatches()).OrderByDescending(dispatch => dispatch.CreatedAt)
            .Select(dispatch => new DispatchResult(dispatch.Id, dispatch.DriverName, dispatch.LicensePlate,
                dispatch.Status, dispatch.CreatedAt,
                assignments.Where(assignment => assignment.DispatchId == dispatch.Id).Select(assignment => assignment.SaleId)))
            .ToArray();
    }
}

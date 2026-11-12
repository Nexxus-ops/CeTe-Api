using CeTe.Logistics.Application.Results;
using CeTe.Sales.Domain.Model.Queries;
using CeTe.Logistics.Domain.Model.Aggregates;
using CeTe.Logistics.Domain.Model.Commands;
using CeTe.Logistics.Domain.Repositories;
using CeTe.Shared.Application.Services;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.Repositories;
using CeTe.Shared.Domain.ValueObjects;
using CeTe.Sales.Application.Services;

using CeTe.Logistics.Application.Services;
namespace CeTe.Logistics.Application.Internal.CommandServices;

public sealed class LogisticsCommandService(ILogisticsRepository repository, IUnitOfWork unitOfWork, ISalesQueryService sales) : ILogisticsCommandService
{
    public Task<DispatchResult> CreateDispatch(CreateDispatchCommand input) => unitOfWork.Transaction<DispatchResult>(async () =>
    {
        Rules.Require(!string.IsNullOrWhiteSpace(input.DriverName) && !string.IsNullOrWhiteSpace(input.LicensePlate) && input.SaleIds.Count > 0, "Driver, plate and sales are required.");
        Rules.Require(input.SaleIds.Distinct().Count() == input.SaleIds.Count, "Duplicate sale.");
        var plate = input.LicensePlate.Trim().ToUpperInvariant();
        Rules.Require(!(await repository.Dispatches()).Any(d => d.Status != "DELIVERED" && (d.LicensePlate == plate || d.DriverName == input.DriverName.Trim())), "Driver or vehicle already assigned.", 409);
        var dispatch = new Dispatch(input.DriverName, plate);
        repository.AddDispatch(dispatch);
        foreach (var id in input.SaleIds)
        {
            var status = await sales.Status(new GetSaleStatusQuery(id));
            Rules.Require(status == "COMPLETED", "Only completed sales can be dispatched.");
            Rules.Require(!await repository.HasSaleAssignment(id), "Sale already assigned.", 409);
            repository.AddAssignment(new DispatchSale { DispatchId = dispatch.Id, SaleId = id });
        }
        await unitOfWork.Save();
        return new DispatchResult(dispatch.Id, dispatch.DriverName, dispatch.LicensePlate, dispatch.Status, dispatch.CreatedAt, input.SaleIds);
    });
    public Task<DispatchStatusResult> SetDispatchStatus(Guid id, ChangeDispatchStatusCommand command) =>
        unitOfWork.Transaction<DispatchStatusResult>(async () =>
        {
            var dispatch = await repository.FindDispatch(id) ?? throw new BusinessException("Resource not found.", 404);
            dispatch.Transition(command.Status);
            return new DispatchStatusResult(dispatch.Id, dispatch.DriverName, dispatch.LicensePlate, dispatch.Status, dispatch.CreatedAt);
        });

    public Task<DispatchIncident> Incident(Guid id, RegisterIncidentCommand command) =>
        unitOfWork.Transaction<DispatchIncident>(async () =>
        {
            var dispatch = await repository.FindDispatch(id) ?? throw new BusinessException("Resource not found.", 404);
            Rules.Require(!string.IsNullOrWhiteSpace(command.Description), "Description is required.");
            dispatch.Transition("INCIDENT");
            var incident = new DispatchIncident { DispatchId = id, Description = command.Description.Trim() };
            repository.AddIncident(incident);
            return incident;
        });
}

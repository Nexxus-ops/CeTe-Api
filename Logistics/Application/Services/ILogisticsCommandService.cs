using CeTe.Logistics.Domain.Model.Aggregates;
using CeTe.Logistics.Application.Results;
using CeTe.Logistics.Domain.Model.Commands;
using CeTe.Sales.Application.Services;
namespace CeTe.Logistics.Application.Services;

public interface ILogisticsCommandService
{
    Task<DispatchResult> CreateDispatch(CreateDispatchCommand input);
    Task<DispatchStatusResult> SetDispatchStatus(Guid id, ChangeDispatchStatusCommand command);
    Task<DispatchIncident> Incident(Guid id, RegisterIncidentCommand command);
}

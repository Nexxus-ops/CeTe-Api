using CeTe.Logistics.Domain.Model.Aggregates;

namespace CeTe.Logistics.Domain.Repositories;

public interface ILogisticsRepository
{
    Task<List<Dispatch>> Dispatches();
    Task<Dispatch?> FindDispatch(Guid id);
    Task<List<DispatchSale>> Assignments();
    Task<bool> HasSaleAssignment(Guid id);
    void AddDispatch(Dispatch dispatch);
    void AddAssignment(DispatchSale assignment);
    void AddIncident(DispatchIncident incident);
}

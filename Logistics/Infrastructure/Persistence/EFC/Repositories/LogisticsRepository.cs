using CeTe.Logistics.Domain.Model.Aggregates;
using CeTe.Logistics.Domain.Repositories;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using CeTe.Shared.Infrastructure.Persistence.EFC.Repositories;

namespace CeTe.Logistics.Infrastructure.Persistence.EFC.Repositories;

public sealed class LogisticsRepository(CeTeDb db) : BaseRepository(db), ILogisticsRepository
{
    public Task<List<Dispatch>> Dispatches() => List<Dispatch>();
    public Task<Dispatch?> FindDispatch(Guid id) => Find<Dispatch>(id);
    public Task<List<DispatchSale>> Assignments() => List<DispatchSale>();
    public Task<bool> HasSaleAssignment(Guid id) => SaleAssigned(id);
    public void AddDispatch(Dispatch dispatch) => Add(dispatch);
    public void AddAssignment(DispatchSale assignment) => Add(assignment);
    public void AddIncident(DispatchIncident incident) => Add(incident);

    private async Task<bool> SaleAssigned(Guid id) =>
        (await List<DispatchSale>(assignment => assignment.SaleId == id)).Count != 0;
}

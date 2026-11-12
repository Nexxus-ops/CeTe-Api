using CeTe.Logistics.Domain.Repositories;
using CeTe.Sales.Application.Services;
namespace CeTe.Logistics.Infrastructure.ACL;

public sealed class SaleDispatchPolicy(ILogisticsRepository repository) : ISaleDispatchPolicy
{
    public Task<bool> HasAssignment(Guid saleId) => repository.HasSaleAssignment(saleId);
}

namespace CeTe.Sales.Application.Services;

public interface ISaleDispatchPolicy
{
    Task<bool> HasAssignment(Guid saleId);
}

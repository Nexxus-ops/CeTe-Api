namespace CeTe.Shared.Domain.Repositories;

public interface IUnitOfWork
{
    Task Save();
    Task<T> Transaction<T>(Func<Task<T>> action);
}

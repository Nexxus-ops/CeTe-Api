namespace CeTe.Shared.Domain.Model;

public abstract class Entity
{
    public Guid Id { get; set; } = Guid.NewGuid();
}

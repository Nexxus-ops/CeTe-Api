using CeTe.Shared.Domain.Model;

namespace CeTe.Logistics.Domain.Model.Aggregates;

public sealed class DispatchStop : TenantEntity
{
    public Guid DispatchId
    {
        get; set;
    }
    public string Address { get; set; } = ""; public int Sequence
    {
        get; set;
    }
}

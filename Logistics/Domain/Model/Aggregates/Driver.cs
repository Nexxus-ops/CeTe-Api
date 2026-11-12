using CeTe.Shared.Domain.Model;

namespace CeTe.Logistics.Domain.Model.Aggregates;

public sealed class Driver : TenantEntity
{
    public string Name { get; set; } = ""; public string License { get; set; } = "";
}

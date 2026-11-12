using CeTe.Shared.Domain.Model;

namespace CeTe.Procurement.Domain.Model.Aggregates;

public sealed class Supplier : TenantEntity
{
    public string Name { get; set; } = ""; public string Contact { get; set; } = "";
}

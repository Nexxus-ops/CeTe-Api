using CeTe.Shared.Domain.Model;

namespace CeTe.Sales.Domain.Model.Aggregates;

public sealed class Customer : TenantEntity
{
    public string Name { get; set; } = ""; public string Contact { get; set; } = "";
}

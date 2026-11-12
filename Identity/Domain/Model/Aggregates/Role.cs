using CeTe.Shared.Domain.Model;

namespace CeTe.Identity.Domain.Model.Aggregates;

public sealed class Role : TenantEntity
{
    public string Name { get; set; } = "";
}

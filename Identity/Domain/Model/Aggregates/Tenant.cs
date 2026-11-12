using CeTe.Shared.Domain.Model;

namespace CeTe.Identity.Domain.Model.Aggregates;

public sealed class Tenant : Entity
{
    public string Name { get; set; } = ""; public string Ruc { get; set; } = ""; public string Address { get; set; } = "";
}

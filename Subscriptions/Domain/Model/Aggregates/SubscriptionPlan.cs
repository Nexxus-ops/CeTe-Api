using CeTe.Shared.Domain.Model;

namespace CeTe.Subscriptions.Domain.Model.Aggregates;

public sealed class SubscriptionPlan : Entity
{
    public string Name { get; set; } = ""; public decimal MonthlyPrice
    {
        get; set;
    }
}

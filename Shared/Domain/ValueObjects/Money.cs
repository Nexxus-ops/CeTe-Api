namespace CeTe.Shared.Domain.ValueObjects;

public sealed record Money
{
    public decimal Amount
    {
        get;
    }
    public Money(decimal amount)
    {
        Rules.Require(amount >= 0, "Invalid quantities or price.");
        Amount = Rules.Money(amount);
    }
}

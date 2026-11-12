using CeTe.Shared.Domain.Model;

namespace CeTe.Shared.Domain.ValueObjects;

public static class Rules
{
    public static void Require(bool condition, string message, int status = 422)
    {
        if (!condition)
            throw new BusinessException(message, status);
    }
    public static decimal Money(decimal amount) => decimal.Round(amount, 2, MidpointRounding.AwayFromZero);
}

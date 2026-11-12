namespace CeTe.Shared.Domain.Model;

public sealed class BusinessException(string message, int status = 422) : Exception(message)
{
    public int Status { get; } = status;
}

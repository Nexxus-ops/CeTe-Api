namespace CeTe.Reporting.Application.Services;

public record LowStockResult(Guid Id, string Name, decimal MinimumStock, decimal Quantity);
public record RevenueTrendResult(string Date, decimal Total);
public record DashboardResult(
    decimal DailyRevenue, int DailySales, int ProductCount,
    IEnumerable<LowStockResult> LowStock, IEnumerable<RevenueTrendResult> Trends, int PendingDispatches);
public interface IReportingQueryService
{
    Task<DashboardResult> Dashboard();
}

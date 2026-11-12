using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Logistics.Domain.Model.Aggregates;
using CeTe.Reporting.Application.Services;
using CeTe.Sales.Domain.Model.Aggregates;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using Microsoft.EntityFrameworkCore;
namespace CeTe.Reporting.Infrastructure.Persistence.EFC;
// Cross-context reporting is a read projection. Tenant filters remain enabled.
public sealed class ReportingQueryService(CeTeDb db) : IReportingQueryService
{
    public async Task<DashboardResult> Dashboard()
    {
        var sales = await db.Set<Sale>().AsNoTracking().Where(sale => sale.Status == "COMPLETED").ToListAsync();
        var balances = await db.Set<StockBalance>().AsNoTracking().ToListAsync();
        var products = await db.Set<Product>().AsNoTracking().Where(product => product.Active).ToListAsync();
        var today = DateTimeOffset.UtcNow.ToOffset(TimeSpan.FromHours(-5)).Date;
        var dailySales = sales.Where(sale => sale.CreatedAt.ToOffset(TimeSpan.FromHours(-5)).Date == today).ToList();
        var lowStock = products
            .Where(product => balances.Where(balance => balance.ProductId == product.Id).Sum(balance => balance.Quantity) <= product.MinimumStock)
            .Select(product => new LowStockResult(product.Id, product.Name, product.MinimumStock,
                balances.Where(balance => balance.ProductId == product.Id).Sum(balance => balance.Quantity)));
        var trends = sales.GroupBy(sale => sale.CreatedAt.ToOffset(TimeSpan.FromHours(-5)).ToString("yyyy-MM-dd"))
            .OrderBy(group => group.Key).TakeLast(14)
            .Select(group => new RevenueTrendResult(group.Key, group.Sum(sale => sale.TotalAmount)));
        var pending = await db.Set<Dispatch>().CountAsync(dispatch => dispatch.Status != "DELIVERED");
        return new DashboardResult(dailySales.Sum(sale => sale.TotalAmount), dailySales.Count,
            products.Count, lowStock, trends, pending);
    }
}

using System.Data;
using System.Reflection;
using CeTe.Application;
using CeTe.Domain;
using Microsoft.EntityFrameworkCore;

namespace CeTe.Infrastructure;

public sealed class CeTeDb(DbContextOptions<CeTeDb> options, ICurrentUser current) : DbContext(options), ICeTeRepository
{
    public Guid CurrentTenantId => current.TenantId;
    public static Type[] EntityTypes => typeof(Entity).Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(Entity).IsAssignableFrom(t)).ToArray();
    protected override void OnModelCreating(ModelBuilder model)
    {
        model.Ignore<Entity>();
        model.Ignore<TenantEntity>();
        foreach (var type in EntityTypes)
        {
            var table = type.Name switch
            {
                "Category" => "Categories",
                "Dispatch" => "Dispatches",
                _ => type.Name + "s"
            };
            var e = model.Entity(type);
            e.HasBaseType((Type?)null);
            e.ToTable(table);
            e.HasKey("Id");
            e.Property("Id").ValueGeneratedNever();
            foreach (var p in type.GetProperties().Where(p => p.PropertyType == typeof(decimal)))
                e.Property(p.Name).HasPrecision(18, 2);
            if (typeof(TenantEntity).IsAssignableFrom(type))
            {
                e.HasAlternateKey("TenantId", "Id");
                e.HasOne(typeof(Tenant)).WithMany().HasForeignKey("TenantId").OnDelete(DeleteBehavior.Restrict);
                GetType().GetMethod(nameof(Filter), BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(type).Invoke(this, [model]);
            }
        }
        foreach (var type in EntityTypes.Where(t => typeof(TenantEntity).IsAssignableFrom(t)))
            foreach (var p in type.GetProperties().Where(p => p.PropertyType == typeof(Guid) && p.Name.EndsWith("Id") && p.Name is not "TenantId" and not "Id"))
            {
                var principal = EntityTypes.FirstOrDefault(t => t.Name == p.Name[..^2]);
                if (principal == null)
                    continue;
                var e = model.Entity(type);
                if (typeof(TenantEntity).IsAssignableFrom(principal))
                    e.HasOne(principal).WithMany().HasForeignKey("TenantId", p.Name).HasPrincipalKey("TenantId", "Id").OnDelete(DeleteBehavior.Restrict);
                else
                    e.HasOne(principal).WithMany().HasForeignKey(p.Name).OnDelete(DeleteBehavior.Restrict);
            }
        model.Entity<User>().HasIndex(x => x.Username).IsUnique();
        model.Entity<Tenant>().HasIndex(x => x.Ruc).IsUnique();
        model.Entity<Product>().HasIndex(x => new { x.TenantId, x.Sku }).IsUnique();
        model.Entity<StockBalance>().HasIndex(x => new { x.TenantId, x.ProductId, x.WarehouseId }).IsUnique();
        model.Entity<SalesReceipt>().HasIndex(x => new { x.TenantId, x.SaleId }).IsUnique();
        model.Entity<SalesReceipt>().HasIndex(x => new { x.TenantId, x.Number }).IsUnique();
        model.Entity<DispatchSale>().HasIndex(x => new { x.TenantId, x.SaleId }).IsUnique();
        model.Entity<Notification>().HasIndex(x => new { x.TenantId, x.EventKey }).IsUnique();
    }
    private void Filter<T>(ModelBuilder model) where T : TenantEntity => model.Entity<T>().HasQueryFilter(e => e.TenantId == CurrentTenantId);
    public Task<List<T>> List<T>(System.Linq.Expressions.Expression<Func<T, bool>>? predicate = null) where T : Entity => predicate == null ? Set<T>().ToListAsync() : Set<T>().Where(predicate).ToListAsync();
    public Task<T?> Find<T>(Guid id) where T : Entity => Set<T>().SingleOrDefaultAsync(e => e.Id == id);
    void ICeTeRepository.Add<T>(T item)
    {
        if (item is TenantEntity e && e.TenantId == Guid.Empty)
            e.TenantId = CurrentTenantId;
        Set<T>().Add(item);
    }
    public async Task Save() => await SaveChangesAsync();
    public async Task<T> Transaction<T>(Func<Task<T>> action)
    {
        await using var transaction = await Database.BeginTransactionAsync(IsolationLevel.Serializable);
        var result = await action();
        await SaveChangesAsync();
        await transaction.CommitAsync();
        return result;
    }
}

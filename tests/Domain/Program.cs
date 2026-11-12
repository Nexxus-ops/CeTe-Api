using CeTe.Shared.Domain.Model;
using CeTe.Shared.Application.Services;
using CeTe.Shared.Infrastructure.Persistence.EFC.Configuration;
using CeTe.Inventory.Domain.Model.ValueObjects;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using CeTe.Shared.Domain.ValueObjects;
using CeTe.Inventory.Domain.Model.Aggregates;
using CeTe.Logistics.Domain.Model.Aggregates;
using CeTe.Procurement.Domain.Model.Aggregates;
using CeTe.Sales.Domain.Model.Aggregates;
static void Reject(Action operation)
{
    try
    {
        operation();
    }
    catch (BusinessException)
    {
        return;
    }
    throw new Exception("Expected a rejected domain operation.");
}

static void Equal(decimal expected, decimal actual)
{
    if (expected != actual)
        throw new Exception($"Expected {expected}, got {actual}.");
}

var stock = new StockBalance();
Reject(() => stock.Receive(0));
Reject(() => stock.Receive(-1));
stock.Receive(10);
stock.Remove(4);
Equal(6, stock.Quantity);
Reject(() => stock.Remove(7));
Equal(6, stock.Quantity);
Reject(() => stock.Remove(-1));

var sale = new Sale(Guid.NewGuid(), "Test", Guid.NewGuid(), Guid.NewGuid(), "hash");
Reject(() => sale.Confirm([]));
sale.Confirm([new SaleLine(sale.Id, Guid.NewGuid(), "SKU", "Product", 2, new Money(12.50m))]);
Equal(25, sale.TotalAmount);
Reject(() => sale.Confirm([new SaleLine(sale.Id, Guid.NewGuid(), "SKU", "Product", 1, new Money(1))]));
sale.Cancel();
Reject(sale.Cancel);
Reject(() => new SaleLine(Guid.NewGuid(), Guid.NewGuid(), "SKU", "Product", -1, new Money(2)));

var dispatch = new Dispatch("Test driver", "ABC-123");
Reject(() => dispatch.Transition("DELIVERED"));
dispatch.Transition("IN_TRANSIT");
dispatch.Transition("INCIDENT");
dispatch.Transition("IN_TRANSIT");
dispatch.Transition("DELIVERED");
Reject(() => dispatch.Transition("PENDING"));

var sku = new Sku(" sku-01 ");
if (sku.Value != "SKU-01")
    throw new Exception("SKU normalization changed.");
Reject(() => new Sku(" "));
Reject(() => new Money(-1));
var product = new Product();
product.UpdateDetails(sku, " Product ", new Money(12.505m), 3);
Equal(12.51m, product.UnitPrice);
Reject(() => product.Archive(true));

var order = new PurchaseOrder(Guid.NewGuid(), Guid.NewGuid());
var purchaseLine = new PurchaseOrderLine(order.Id, Guid.NewGuid(), 10, new Money(4));
order.Receive(purchaseLine, 4);
Reject(() => order.Receive(purchaseLine, 7));
Equal(4, purchaseLine.ReceivedQuantity);
order.Receive(purchaseLine, 6);
if (order.Status != "RECEIVED")
    throw new Exception("The completed order must be received.");
Reject(() => order.Receive(purchaseLine, 1));
Console.WriteLine("PASS: stock, sales, cancellation, dispatch transitions, SKU, money and receipt invariants.");

// An unchanged relational model is required to open databases created before the
// namespace and encapsulation refactor.
var root = new DirectoryInfo(AppContext.BaseDirectory);
while (root != null && !File.Exists(Path.Combine(root.FullName, "CeTe.Api.csproj")))
    root = root.Parent;
if (root == null) throw new Exception("API source root was not found.");
var sql = File.ReadAllText(Path.Combine(root.FullName, "schema.sql"));
var options = new DbContextOptionsBuilder<CeTeDb>()
    .UseNpgsql("Host=localhost;Database=model_inspection;Username=unused;Password=unused").Options;
using var db = new CeTeDb(options, new TestCurrentUser());
var tables = db.Model.GetRelationalModel().Tables.ToDictionary(table => table.Name);
var expectedTables = Regex.Matches(sql, """CREATE TABLE public\."([^"]+)" \((.*?)\n\);""", RegexOptions.Singleline);
if (tables.Count != 35 || expectedTables.Count != 35) throw new Exception("Expected the existing 35 tables.");
foreach (Match table in expectedTables)
{
    var name = table.Groups[1].Value;
    var columns = Regex.Matches(table.Groups[2].Value, """^    "([^"]+)" ([^\r\n]+)""", RegexOptions.Multiline)
        .Select(match => match.Groups[1].Value).Order().ToArray();
    if (!tables.TryGetValue(name, out var actual) || !columns.SequenceEqual(actual.Columns.Select(column => column.Name).Order()))
        throw new Exception($"The existing columns changed in {name}.");
}
if (tables.Values.Sum(table => table.ForeignKeyConstraints.Count()) != 67)
    throw new Exception("Existing tenant foreign keys changed.");
foreach (var folder in new[] { "Inventory", "Sales", "Procurement", "Logistics", "Identity" })
{
    foreach (var file in Directory.EnumerateFiles(Path.Combine(root.FullName, folder, "Application"), "*.cs", SearchOption.AllDirectories))
    {
        var source = File.ReadAllText(file);
        if (Regex.IsMatch(source, @"using (?:CeTe\.[^;]*Infrastructure|Microsoft\.EntityFrameworkCore)"))
            throw new Exception($"Application depends on persistence: {file}");
    }
}
var sqliteOptions = new DbContextOptionsBuilder<CeTeDb>().UseSqlite("Data Source=:memory:").Options;
using (var sqliteDb = new CeTeDb(sqliteOptions, new TestCurrentUser()))
{
    sqliteDb.Database.OpenConnection();
    sqliteDb.Database.EnsureCreated();
    if (sqliteDb.Model.GetRelationalModel().Tables.Count() != 35)
        throw new Exception("SQLite development schema changed.");
}
Console.WriteLine("PASS: value objects, aggregate receipt rules, 35-table column compatibility, 67 foreign keys and application boundaries.");

sealed class TestCurrentUser : ICurrentUser
{
    public Guid TenantId => Guid.Empty;
    public Guid UserId => Guid.Empty;
    public string Username => "model-inspection";
}

using CeTe.Sales.Application.Results;
using CeTe.Sales.Domain.Model.Queries;
using CeTe.Sales.Domain.Model.Aggregates;
using CeTe.Sales.Domain.Model.Commands;
using CeTe.Sales.Domain.Repositories;
using CeTe.Shared.Application.Services;
using CeTe.Shared.Domain.Model;
using CeTe.Shared.Domain.Repositories;
using CeTe.Shared.Domain.ValueObjects;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CeTe.Inventory.Application.Services;
using CeTe.Identity.Application.Services;


using CeTe.Sales.Application.Services;
namespace CeTe.Sales.Application.Internal.CommandServices;

public sealed class SalesCommandService(ISalesRepository repository, IUnitOfWork unitOfWork, ICurrentUser user, IInventoryFacade inventory, ICompanyQueryService companies, ISalesQueryService queries, ISaleDispatchPolicy dispatches) : ISalesCommandService
{
    public Task<SaleResult> Checkout(ConfirmSaleCommand input) => unitOfWork.Transaction<SaleResult>(async () =>
    {
        Rules.Require(input.Id != Guid.Empty, "An idempotency identifier is required.");
        Rules.Require(input.Items.Count > 0 && input.Items.Select(i => i.InventoryItemId).Distinct().Count() == input.Items.Count, "Use one line per product.");
        Rules.Require(new[] { "CASH", "CARD", "TRANSFER" }.Contains(input.PaymentMethod), "Unsupported payment method.");
        var hash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(input))));
        var existing = await repository.FindSale(input.Id);
        if (existing != null)
        {
            Rules.Require(existing.RequestHash == hash, "This identifier belongs to a different request.", 409);
            return await queries.SaleView(new GetSaleByIdQuery(existing.Id));
        }
        var warehouseId = (await inventory.GetWarehouse(input.WarehouseId)).Id;
        var sale = new Sale(input.Id, input.CustomerName, warehouseId, user.UserId, hash);
        var lines = new List<SaleLine>();
        foreach (var item in input.Items)
        {
            var product = await inventory.GetProduct(item.InventoryItemId);
            Rules.Require(product.Active, "The product is archived.");
            Rules.Require(item.Quantity > 0, "Quantity must be positive.");
            await inventory.AdjustStock(product.Id, warehouseId, -item.Quantity, $"SALE:{sale.Id}");
            var line = new SaleLine(sale.Id, product.Id, product.Sku, product.Name, item.Quantity, new Money(product.UnitPrice));
            lines.Add(line);
            repository.AddLine(line);
        }
        sale.Confirm(lines);
        repository.AddSale(sale);
        repository.AddPayment(new Payment(sale.Id, input.PaymentMethod, new Money(sale.TotalAmount)));
        var tenant = await companies.CurrentCompany();
        var warehouse = await inventory.GetWarehouse(warehouseId);
        repository.AddReceipt(new SalesReceipt(sale, tenant.Name, tenant.Ruc, warehouse.Address, user.Username));
        await unitOfWork.Save();
        return await queries.SaleView(new GetSaleByIdQuery(sale.Id));
    });
    public Task<SaleResult> CancelSale(Guid id) => unitOfWork.Transaction<SaleResult>(async () =>
    {
        var sale = await repository.FindSale(id) ?? throw new BusinessException("Resource not found.", 404);
        Rules.Require(!await dispatches.HasAssignment(id), "A dispatched sale cannot be cancelled. Register its return instead.", 409);
        sale.Cancel();
        foreach (var line in await repository.Lines(id))
        {
            await inventory.AdjustStock(line.ProductId, sale.WarehouseId, line.Quantity, $"CANCEL:{id}");
        }
        foreach (var payment in await repository.Payments(id))
            payment.Reverse();
        return await queries.SaleView(new GetSaleByIdQuery(sale.Id));
    });
}

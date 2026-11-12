using CeTe.Sales.Domain.Model.Aggregates;
using CeTe.Sales.Application.Results;
using CeTe.Sales.Domain.Model.Commands;

using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using CeTe.Inventory.Application.Services;
using CeTe.Identity.Application.Services;

namespace CeTe.Sales.Application.Services;

public interface ISalesCommandService
{
    Task<SaleResult> Checkout(ConfirmSaleCommand input);
    Task<SaleResult> CancelSale(Guid id);
}

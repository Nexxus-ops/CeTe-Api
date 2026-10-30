using CeTe.Domain;
static void Reject(Action operation) { try { operation(); } catch (BusinessException) { return; } throw new Exception("Expected a rejected domain operation."); }
static void Equal(decimal expected, decimal actual) { if (expected != actual) throw new Exception($"Expected {expected}, got {actual}."); }
var stock = new StockBalance();
Reject(() => stock.Receive(0)); Reject(() => stock.Receive(-1));
stock.Receive(10); stock.Remove(4); Equal(6, stock.Quantity);
Reject(() => stock.Remove(7)); Equal(6, stock.Quantity);
Reject(() => stock.Remove(-1));
var sale = new Sale(); Reject(() => sale.Confirm([]));
sale.Confirm([new SaleLine { Quantity = 2, UnitPrice = 12.50m }]); Equal(25, sale.TotalAmount);
Reject(() => new Sale().Confirm([new SaleLine { Quantity = -1, UnitPrice = 2 }]));
sale.Cancel(); Reject(sale.Cancel);
var dispatch = new Dispatch(); Reject(() => dispatch.Transition("DELIVERED"));
dispatch.Transition("IN_TRANSIT"); dispatch.Transition("INCIDENT"); dispatch.Transition("IN_TRANSIT"); dispatch.Transition("DELIVERED"); Reject(() => dispatch.Transition("PENDING"));
Console.WriteLine("PASS: stock invariants, monetary totals, cancellation and dispatch state transitions.");

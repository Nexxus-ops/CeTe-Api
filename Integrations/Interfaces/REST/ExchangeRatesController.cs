using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace CeTe.Integrations.Interfaces.REST;

[ApiController]
[Authorize]
[Route("api/v1/integrations")]
[Tags("Integrations")]
public sealed class ExchangeRatesController(IHttpClientFactory factory) : ControllerBase
{
    [HttpGet("exchange-rates")]
    public async Task<IActionResult> GetExchangeRates()
    {
        try
        {
            var response = await factory.CreateClient("rates").GetFromJsonAsync<System.Text.Json.JsonElement>("v6/latest/USD");
            return Ok(new
            {
                source = "ExchangeRate-API",
                sourceUrl = "https://www.exchangerate-api.com",
                baseCurrency = "USD",
                pen = response.GetProperty("rates").GetProperty("PEN").GetDecimal(),
                updatedAt = response.GetProperty("time_last_update_utc").GetString()
            });
        }
        catch (Exception error) when (error is HttpRequestException or TaskCanceledException or System.Text.Json.JsonException)
        {
            return Problem("Exchange rate provider is unavailable.", statusCode: 503);
        }
    }
}

using CeTe.Shared.Domain.Model;
using Microsoft.EntityFrameworkCore;
namespace CeTe.Shared.Interfaces.REST;

public sealed class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (BusinessException error)
        {
            context.Response.StatusCode = error.Status;
            await context.Response.WriteAsJsonAsync(new
            {
                message = ErrorLanguage.Translate(error.Message, context)
            });
        }
        catch (Exception error) when (IsConcurrencyConflict(error))
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsJsonAsync(new
            {
                message = ErrorLanguage.Translate("Concurrent operation. Refresh and retry.", context)
            });
        }
        catch (DbUpdateException)
        {
            context.Response.StatusCode = 409;
            await context.Response.WriteAsJsonAsync(new
            {
                message = ErrorLanguage.Translate("The operation conflicts with current data. Refresh and retry.", context)
            });
        }
        catch (Exception error) when (error is not OperationCanceledException)
        {
            logger.LogError(error, "Request failed");
            context.Response.StatusCode = 500;
            await context.Response.WriteAsJsonAsync(new
            {
                message = ErrorLanguage.Translate("The operation could not be completed.", context)
            });
        }
    }

    private static bool IsConcurrencyConflict(Exception error)
    {
        for (Exception? current = error; current != null; current = current.InnerException)
            if (current is Npgsql.PostgresException { SqlState: "40001" or "40P01" })
                return true;
        return false;
    }
}

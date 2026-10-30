namespace CeTe.Infrastructure;

public static class ErrorLanguage
{
    private static readonly Dictionary<string,string> Spanish = new()
    {
        ["Resource not found."] = "No se encontró el recurso.",
        ["Warehouse not found."] = "No se encontró el almacén.",
        ["Quantity must be positive."] = "La cantidad debe ser positiva.",
        ["Insufficient stock."] = "No hay existencias suficientes.",
        ["SKU already exists."] = "El SKU ya está registrado.",
        ["SKU and name are required."] = "El SKU y el nombre son obligatorios.",
        ["Invalid quantities or price."] = "Las cantidades o el precio no son válidos.",
        ["A movement reason is required."] = "El motivo del movimiento es obligatorio.",
        ["Record an exit before archiving stock."] = "Registra una salida antes de archivar el producto.",
        ["An idempotency identifier is required."] = "Se requiere un identificador de operación.",
        ["Use one line per product."] = "Utiliza una línea por producto.",
        ["Unsupported payment method."] = "El método de pago no está admitido.",
        ["This identifier belongs to a different request."] = "El identificador pertenece a otra operación.",
        ["The product is archived."] = "El producto está archivado.",
        ["The sale is empty."] = "La venta no tiene productos.",
        ["Invalid sale lines."] = "El detalle de venta no es válido.",
        ["Sale already cancelled."] = "La venta ya está anulada.",
        ["A dispatched sale cannot be cancelled. Register its return instead."] = "La venta tiene un despacho asociado y no puede anularse.",
        ["Receipt not found."] = "No se encontró la boleta.",
        ["Driver, plate and sales are required."] = "El conductor, la placa y las ventas son obligatorios.",
        ["Duplicate sale."] = "La venta está duplicada.",
        ["Driver or vehicle already assigned."] = "El conductor o vehículo ya está asignado.",
        ["Only completed sales can be dispatched."] = "Solo se pueden despachar ventas completadas.",
        ["Sale already assigned."] = "La venta ya tiene un despacho.",
        ["Invalid dispatch transition."] = "El cambio de estado del despacho no es válido.",
        ["Description is required."] = "La descripción es obligatoria.",
        ["Invalid purchase."] = "Los datos de compra no son válidos.",
        ["Purchase already received."] = "La compra ya fue recibida.",
        ["Quantity exceeds the pending receipt."] = "La cantidad supera la recepción pendiente.",
        ["Enter a valid email address."] = "Ingresa un correo electrónico válido.",
        ["Password must contain between 10 and 128 characters."] = "La contraseña debe tener entre 10 y 128 caracteres.",
        ["Company, address and an 11-digit RUC are required."] = "Se requieren empresa, dirección y RUC de 11 dígitos.",
        ["The account or company is already registered."] = "La cuenta o empresa ya está registrada.",
        ["Invalid credentials."] = "Las credenciales no son válidas.",
        ["Invalid employee role."] = "El rol del trabajador no es válido.",
        ["Account already registered."] = "La cuenta ya está registrada.",
        ["Concurrent operation. Refresh and retry."] = "Otra operación modificó los datos. Actualiza y vuelve a intentar.",
        ["The operation conflicts with current data. Refresh and retry."] = "La operación entra en conflicto con los datos actuales. Actualiza y vuelve a intentar.",
        ["The operation could not be completed."] = "No se pudo completar la operación."
    };
    public static string Translate(string message, HttpContext context) => context.Request.Headers.AcceptLanguage.ToString().StartsWith("es", StringComparison.OrdinalIgnoreCase) && Spanish.TryGetValue(message, out var translated) ? translated : message;
}

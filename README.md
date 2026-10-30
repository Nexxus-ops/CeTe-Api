# CeTe API · AV2

API propia de C# / ASP.NET Core 10, Entity Framework Core y PostgreSQL 17. Arquitectura inicial con entidades e invariantes en `Domain`, casos de uso y puertos en `Application`, y adaptadores de persistencia y autenticación en `Infrastructure`.

## Ejecutar con Docker

Desde este repositorio, ejecutar `./Initialize-Local.ps1` una sola vez: crea `.env` con secretos aleatorios locales y no los muestra. Después:

```powershell
docker compose up -d --build
```

- Swagger: http://localhost:5080/swagger
- Salud y conexión a PostgreSQL: http://localhost:5080/health
- API: http://localhost:5080/api/v1

Crear una empresa desde el formulario de registro del frontend. No hay usuarios ni contraseñas predeterminados. La primera cuenta es administradora. Desde Equipo se crean cuentas Warehouse, Sales o Logistics. Las credenciales de prueba se generan solo en memoria.

Docker Compose conserva PostgreSQL en un volumen. `docker compose stop` detiene los servicios conservando datos. No eliminar el volumen para actualizar la aplicación.

## Alcance implementado

- Registro, autenticación con hash de contraseña, JWT con vencimiento y sesiones revocables.
- Empresa separada por tenant: filtros y claves foráneas compuestas impiden cruzar recursos entre empresas.
- Catálogo, SKU único, stock por almacén, entradas, salidas y movimientos auditables.
- Órdenes de compra de un producto y recepción física parcial; crear la orden no suma stock.
- Venta con precios calculados en servidor y transacción serializable: stock, venta, líneas, pago registrado y boleta se confirman juntos.
- Reintento de venta con UUID idempotente. Anulación que repone stock una vez y revierte el pago registrado.
- Boleta académica persistente con empresa, dirección, vendedor, orden, fecha/hora, productos, cantidades, precios y total. Conserva los datos históricos aunque cambie el catálogo. No es facturación electrónica SUNAT ni procesa tarjetas.
- Despachos, ventas exclusivas por despacho, recursos ocupados, transiciones e incidencias.
- Dashboard basado en datos reales de la empresa y consulta externa de tipo de cambio USD/PEN con atribución a ExchangeRate-API.

`schema.sql` es la exportación del esquema real: **35 tablas**, claves e índices. Las tablas adicionales de lotes, variantes, ubicaciones, devoluciones, intentos de entrega y suscripciones preparan el modelo; sus flujos completos siguen en backlog. Tener tablas no implica tener implementados todos esos módulos.

## Contrato

Swagger es la referencia ejecutable. Los recursos protegidos requieren `Authorization: Bearer <token>`. `Accept-Language: es-419` traduce errores de negocio; inglés por defecto. Las operaciones actuales de creación devuelven 200 con el recurso, cierre de sesión 204, validación 422, conflicto/stock/concurrencia 409, autenticación 401 y autorización 403. Una referencia de otra empresa se responde 404.

Rutas principales: `authentication/sign-up`, `authentication/sign-in`, `authentication/sign-out`, `inventory`, `inventory/{id}/entries`, `inventory/{id}/exits`, `inventory/{id}/history`, `sales`, `sales/{id}/cancel` (PATCH), `sales/{id}/receipt`, `dispatches`, `purchases`, `purchases/{id}/receive`, `users`, `analytics/dashboard`, `integrations/exchange-rates`. Prefijo `/api/v1`.

## Verificar

Con Docker ejecutándose:

```powershell
python tests/api_smoke.py
dotnet run --project tests/Domain/DomainChecks.csproj
```

Las pruebas HTTP crean empresas ficticias nuevas y verifican aislamiento, permisos, recepción parcial, rollback, idempotencia, conservación de boleta, anulación y revocación. No ejecutar contra datos reales de producción. La evidencia del navegador verifica el recorrido registro → acceso → venta → boleta → cambio de idioma → recarga y vista móvil.

## Publicar en Render

`render.yaml` prepara una API Docker y una base PostgreSQL separada. Render aloja el contenedor; Docker permite ejecutar la misma aplicación localmente. Configura una clave JWT generada por Render, conexión desde la base y CORS para GitHub Pages, sin guardar secretos en Git.

1. Crear un Blueprint desde este repositorio y revisar el plan de ambos recursos antes de confirmar.
2. Verificar `/health` y `/swagger` en la URL pública de la API.
3. Configurar la variable de repositorio del frontend `VITE_PLATFORM_API_URL` con `https://<servicio>.onrender.com/api/v1` y publicar el frontend.
4. Probar el recorrido completo desde el frontend público y registrar URLs, commit, fecha y capturas en el informe.

La configuración Free es para demostración: Render documenta suspensión tras 15 minutos de inactividad y vencimiento de PostgreSQL gratuito a los 30 días. Revisar la fecha de sustentación y exportar respaldos antes del vencimiento. Fuente: [Render Free](https://render.com/docs/free), [Blueprint](https://render.com/docs/blueprint-spec).

El esquema inicial usa `EnsureCreated` para una base vacía. No aplica cambios incrementales a bases existentes. Antes de evolucionar una base con información real, preparar migraciones EF y respaldo; no recrearla ni borrar volúmenes. Pendientes de endurecimiento: paginación, migraciones versionadas, recuperación de contraseña y monitoreo. AV2 aún requiere publicación pública y evidencias humanas del curso.

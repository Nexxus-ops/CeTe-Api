<div align="center">
  <h1> CeTe by Nexxus - RESTful API</h1>
  <p><em>El sistema nervioso central de tu logística MYPE</em></p>

  ![C#](https://img.shields.io/badge/C%23-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
  ![ASP.NET Core](https://img.shields.io/badge/ASP.NET_Core-10-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)
  ![PostgreSQL](https://img.shields.io/badge/PostgreSQL-17-4169E1?style=for-the-badge&logo=postgresql&logoColor=white)
  ![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)
  ![Despliegue](https://img.shields.io/badge/Deploy-Render-000000?style=for-the-badge&logo=render&logoColor=white)
</div>

---

## Sobre el Proyecto

Este repositorio contiene "el **RESTful API de CeTe**", una plataforma SaaS B2B desarrollada por el equipo **Nexxus** para centralizar las operaciones logísticas de las MYPES peruanas.

La API proporciona autenticación, reglas de negocio y persistencia para inventario, abastecimiento, ventas, boletas y despachos. 
El incremento AV2 utiliza **C# con ASP.NET Core, Entity Framework Core y PostgreSQL**, con ejecución local y publicación mediante Docker.

**Documentación pública:** [CeTe API - Swagger](https://cete-api-av2.onrender.com/swagger).
**Estado del servicio:** [CeTe API - Health](https://cete-api-av2.onrender.com/health).

## Características Principales

* **Identidad y Acceso:** Registro de empresa, contraseñas protegidas mediante hash, autenticación JWT, vencimiento y revocación de sesiones.
* **Aislamiento por Empresa:** Consultas filtradas por *tenant* y claves foráneas compuestas para mantener separadas las operaciones de cada empresa.
* **Inventario con Trazabilidad:** Catálogo, SKU único por empresa, stock por almacén, entradas, salidas y movimientos auditables.
* **Compras y Recepción Parcial:** Órdenes a proveedores y registro de entregas físicas, con control del saldo pendiente y actualización de stock al recibir.
* **Ventas Atómicas:** Confirmación conjunta de venta, detalle, stock, pago registrado y boleta, con precios y totales calculados en el servidor.
* **Control de Reintentos y Concurrencia:** UUID idempotente para evitar ventas duplicadas y transacciones serializables para proteger la última unidad disponible.
* **Boletas Históricas:** Comprobante académico persistente con empresa, dirección, orden, fecha y hora, vendedor, cliente, productos, cantidades, precios y total.
* **Anulación de Ventas:** Reposición de stock una sola vez y reversión del pago registrado, con validación de ventas asociadas a despachos.
* **Coordinación de Despachos:** Asociación exclusiva de ventas, validación de recursos ocupados, transiciones de estado e incidencias.
* **Indicadores e Integración Externa:** Métricas de ventas e inventario por empresa y consulta de referencia USD/PEN mediante ExchangeRate-API.
* **Documentación Interactiva:** Swagger con autenticación Bearer y endpoint de salud que comprueba la conexión a PostgreSQL.

## Stack Tecnológico

* **Lenguaje y Framework:** C# y ASP.NET Core 10.
* **Persistencia:** Entity Framework Core 10 con Npgsql.
* **Base de Datos:** PostgreSQL 17.
* **Autenticación:** JWT Bearer y hash de contraseñas.
* **Documentación:** OpenAPI y Swagger UI.
* **Contenedores:** Docker y Docker Compose.
* **Alojamiento Actual:** API Docker y PostgreSQL administrado en Render.
* **Verificación:** Comprobaciones de dominio en C#, pruebas HTTP en Python y flujo integrado con el frontend.

## Arquitectura y Estructura del Proyecto

El backend se organiza como un monolito modular con cinco contextos de negocio activos. Cada contexto contiene sus modelos, contratos, casos de uso y adaptadores HTTP y de persistencia.

| Contexto | Responsabilidad |
| :--- | :--- |
| **Inventory** | Productos, SKU, almacenes, saldos y movimientos. |
| **Procurement** | Proveedores, órdenes de compra y recepciones parciales. |
| **Sales** | Confirmación y anulación de ventas, pagos registrados y boletas históricas. |
| **Logistics** | Asignación de ventas, exclusividad de recursos, estados e incidencias. |
| **Identity** | Empresas, usuarios, credenciales y sesiones. |

Reporting es una proyección de lectura transversal. Integrations contiene el adaptador de tipo de cambio. Subscriptions y Notifications conservan los modelos previstos por el esquema para desarrollos posteriores.

```text
CeTe-Api/
  Inventory/                     # La misma estructura se repite por contexto
    Domain/
      Model/
        Aggregates/              # Modelos y reglas de negocio
        Commands/                # Intenciones de modificación
        Queries/                 # Solicitudes de lectura
        ValueObjects/            # SKU validado
      Repositories/              # Contrato de inventario
    Application/
      Internal/CommandServices/  # Casos de uso que modifican estado
      Internal/QueryServices/    # Consultas
      Services/                  # Contratos y fachada entre contextos
      Results/                   # Proyecciones tipadas
    Infrastructure/
      Persistence/EFC/Repositories/
    Interfaces/
      REST/
        Resources/               # Solicitudes HTTP
        Transform/               # Conversión de recursos a comandos
  Procurement/
  Sales/
  Logistics/
  Identity/
  Reporting/
  Integrations/
  Shared/
    Domain/                      # Identidad base, reglas y unidad de trabajo
    Application/                 # Contrato del usuario actual
    Infrastructure/              # DbContext y mecánica de persistencia
    Interfaces/REST/             # Traducción de errores y middleware
  tests/
    Domain/                      # Invariantes y compatibilidad del modelo
    api_smoke.py                 # Aceptación HTTP
  Program.cs                     # Configuración, DI y pipeline HTTP
  Dockerfile
  compose.yaml
  schema.sql
```

Los contratos de repositorio restringen el acceso a las entidades de cada contexto. Las interacciones entre contextos pasan por fachadas o contratos de aplicación. Por ejemplo, Ventas solicita un ajuste a Inventario y Logística consulta la elegibilidad de una venta. La unidad de trabajo comparte un DbContext y mantiene atómicas las operaciones que abarcan varios contextos.

Los agregados de venta, recepción e inventario encapsulan las reglas de confirmación, cantidades y estados. Los objetos de valor validan SKU e importes. Los controladores conservan las rutas y formas JSON que consume el frontend.

El [ejemplo Catch-Up del curso](https://github.com/upc-pre-202620-1asi0730-ahsp/catch-up-platform-deploy) sirve como referencia de organización de capas, comandos, consultas, repositorios y transformadores.

## Modelo de Datos y Alcance

El archivo `schema.sql` documenta **35 tablas físicas**, con relaciones, claves e índices para identidad, inventario, abastecimiento, ventas, logística, devoluciones y notificaciones.

El incremento implementa los flujos de autenticación, stock, compras y recepciones, ventas y boletas, despachos e indicadores. Los modelos de lotes, variantes, ubicaciones, devoluciones, paradas, intentos de entrega, suscripciones y bandeja de notificaciones preparan ampliaciones cuyos flujos completos siguen en el backlog.

Los métodos de pago actuales registran la operación académica. La integración con una pasarela de tarjetas y la emisión electrónica fiscal quedan fuera del incremento actual.

## Instalación y Ejecución Local

### 1. Requisitos

* **Docker** con soporte de Docker Compose.
* **PowerShell** para ejecutar el inicializador local.
* **Python 3** para las pruebas HTTP.
* **.NET SDK 10** para compilar o ejecutar las comprobaciones de dominio fuera de Docker.

### 2. Preparar el Entorno

Desde la raíz del repositorio, ejecutar una sola vez:

```powershell
./Initialize-Local.ps1
```

El inicializador crea `.env` con valores aleatorios para los secretos locales, conservándolos fuera del control de versiones. `.env.example` documenta la configuración esperada.

### 3. Iniciar API y Base de Datos

```powershell
docker compose up -d --build
```

Accesos locales:

* **Swagger:** [http://localhost:5080/swagger](http://localhost:5080/swagger)
* **API:** `http://localhost:5080/api/v1`
* **Salud y PostgreSQL:** [http://localhost:5080/health](http://localhost:5080/health)

PostgreSQL conserva los datos en un volumen de Docker. Para detener los servicios manteniendo esa información:

```powershell
docker compose stop
```

### 4. Crear una Empresa y su Equipo

Registrar una empresa desde el frontend para crear su primera cuenta administradora. Desde el módulo Equipo se pueden crear cuentas con roles **Warehouse**, **Sales** y **Logistics**.

Cada entorno inicia con sus propios datos. Las cuentas de demostración se crean mediante el flujo de registro.

## Documentación y Contratos del API

Swagger es la referencia ejecutable de los endpoints. Los recursos protegidos utilizan `Authorization: Bearer <token>`. El encabezado `Accept-Language: es-419` permite recibir errores de negocio en español; el idioma predeterminado es inglés.

**Prefijo de recursos:** `/api/v1`.

| Área | Endpoints principales |
| :--- | :--- |
| Identidad | `authentication/sign-up`, `authentication/sign-in`, `authentication/sign-out` |
| Inventario | `inventory`, `inventory/{id}/entries`, `inventory/{id}/exits`, `inventory/{id}/history` |
| Ventas y boletas | `sales`, `sales/{id}/cancel` (PATCH), `sales/{id}/receipt` |
| Abastecimiento | `purchases`, `purchases/{id}/receive` |
| Logística | `dispatches`, `dispatches/{id}/incidents` |
| Equipo y almacenes | `users`, `warehouses` |
| Indicadores e integración | `analytics/dashboard`, `integrations/exchange-rates` |

Las creaciones actuales responden **200** con el recurso, el cierre de sesión **204**, la validación **422**, los conflictos de stock o concurrencia **409**, la autenticación **401** y la autorización **403**. Las referencias a recursos de otra empresa se responden con **404**.

## Verificación de la Versión AV2

Con la API ejecutándose:

```powershell
python tests/api_smoke.py
dotnet run --project tests/Domain/DomainChecks.csproj
```

Las pruebas HTTP crean empresas ficticias y comprueban autenticación, permisos, aislamiento, recepción parcial, rollback, idempotencia, conservación de boletas, anulación, revocación, concurrencia, despachos y la integración externa.

Las comprobaciones de dominio validan stock, redondeo monetario, anulación, transiciones, recepciones, objetos de valor y la compatibilidad de las 35 tablas y sus 67 claves foráneas con el esquema anterior. El repositorio del frontend incluye además el comando `npm run test:e2e` para comprobar el recorrido desde la interfaz.

Las pruebas deben ejecutarse en un entorno de demostración. Las credenciales temporales se generan en memoria.

## Despliegue y Configuración

El **Dockerfile** permite ejecutar la API localmente y en Render. El archivo `render.yaml` describe una API Docker y una base PostgreSQL separada, con conexión y clave JWT configuradas en el host.

Para preparar otro entorno:

1. Crear los recursos desde el repositorio y revisar el plan seleccionado.
2. Configurar conexión de PostgreSQL, clave JWT y orígenes CORS del frontend.
3. Comprobar `/health` y `/swagger` en la URL pública.
4. Configurar `VITE_PLATFORM_API_URL` del frontend con la URL HTTPS de la API.
5. Verificar el recorrido integrado de registro, recepción, venta y boleta.

La configuración de referencia utiliza el plan Free para demostración. Consultar sus límites y vigencia en [Render Free](https://render.com/docs/free), y la estructura de configuración en [Render Blueprints](https://render.com/docs/blueprint-spec).

La inicialización actual utiliza `EnsureCreated` sobre una base vacía. La evolución de bases con información requiere respaldos y migraciones versionadas. Paginación, recuperación de contraseña y monitoreo forman parte de las mejoras pendientes.

## Ecosistema del Proyecto

1. **[Landing Page (Presentación)](https://cete-landing-av2.onrender.com/):** Propuesta de valor y acceso a CeTe. [Repositorio](https://github.com/Nexxus-ops/nexxus-landing-page).
2. **[Frontend (App Transaccional)](https://cete-web-av2.onrender.com/):** SPA con Vue.js y PrimeVue para las operaciones de la empresa. [Repositorio](https://github.com/Nexxus-ops/FrontEnd-CeTe).
3. **[RESTful API (Backend)](https://cete-api-av2.onrender.com/swagger):** Reglas de negocio y persistencia de CeTe. [Repositorio](https://github.com/Nexxus-ops/CeTe-Api).
4. **[Informe del Proyecto](https://github.com/Nexxus-ops/Report-CeTe):** Requisitos, arquitectura, planificación y evidencias académicas.

## Equipo de Desarrollo

Construido para democratizar la tecnología logística de las MYPES peruanas.

* **Desarrollo y Arquitectura:** Equipo Nexxus.

---

*Desarrollado por el equipo Nexxus para el curso de Aplicaciones Web de la Universidad Peruana de Ciencias Aplicadas - 2026.*

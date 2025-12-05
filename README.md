# LayeredArchitecture

API REST en .NET 8 con arquitectura en capas: `API`, `Application`, `Domain` e `Infrastructure`. Implementa operaciones CRUD para Productos, Categorías, Proveedores y Pedidos, usando Dapper para acceso a SQL Server y pruebas (unitarias e integración).

## Resumen

- Arquitectura limpia y separada por responsabilidades:
  - `API`: Controladores y configuración del host (Swagger, DI).
  - `Application`: Servicios, DTOs e interfaces de repositorio (lógica de aplicación).
  - `Domain`: Entidades del dominio (`Product`, `Category`, `Supplier`, `Order`, `OrderDetail`).
  - `Infrastructure`: Implementaciones de repositorios con Dapper y `DapperContext` para `SqlConnection`.
  - `Tests`: Pruebas unitarias e integración.

## Estructura del proyecto

- `API/` — Punto de entrada ASP.NET Core. Registra servicios en `Program.cs`.
- `Application/` — DTOs, interfaces (`IProductRepository`, `ICategoryRepository`, `ISupplierRepository`, `IOrderRepository`) y servicios (`ProductService`, `CategoryService`, `SupplierService`, `OrderService`).
- `Domain/` — Entidades del dominio.
- `Infrastructure/` — `DapperContext` y repositorios (`ProductRepository`, `CategoryRepository`, `SupplierRepository`, `OrderRepository`).
- `Tests/` — Pruebas unitarias e de integración.

Archivos clave:
- `API/Program.cs` — configuración y registro de dependencias.
- `Infrastructure/Database/DapperContext.cs` — crea `SqlConnection` usando `DefaultConnection` de la configuración.
- `Infrastructure/Repositories/*.cs` — consultas SQL con Dapper.
- `Application/Services/*` — mapeo entre entidades y DTOs, reglas de negocio (ej.: `CategoryService` evita duplicados por nombre).

## Tecnologías

- .NET 8
- ASP.NET Core
- Dapper
- Microsoft.Data.SqlClient (SQL Server)
- xUnit (u otra, presente en `Tests`)

## Configuración

1. Configurar la cadena de conexión en `appsettings.json` o variables de entorno usando la key `DefaultConnection`.
2. Restaurar paquetes y compilar:

```bash
dotnet restore
dotnet build
```

3. Ejecutar la API:

```bash
dotnet run --project API
```

4. En desarrollo, Swagger estará disponible en `https://localhost:{port}/swagger`.

## Endpoints principales

- Productos: `GET/POST/PUT/DELETE /api/products`
- Categorías: `GET/POST/PUT/DELETE /api/categories` (creación y actualización validan nombres duplicados)
- Proveedores: `GET/POST/PUT/DELETE /api/suppliers`
- Pedidos: `GET/POST/PUT/DELETE /api/orders` (POST crea orden + detalles en transacción)

## Notas importantes

- Las consultas usan parámetros de Dapper (protección contra SQL injection si se usan correctamente). Revisar la validación de datos en los DTOs si se requiere.
- `OrderRepository` usa transacciones al crear o borrar pedidos y sus detalles.
- Añadir scripts de migración / seed para facilitar pruebas de integración.

## Ejecutar tests

```bash
dotnet test
```
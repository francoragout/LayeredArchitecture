# Arquitectura en Capas

<!-- Badges / Iconos -->

<img src="https://img.shields.io/badge/.NET-8-512BD4?logo=dotnet&logoColor=white" alt=".NET 8" />
<img src="https://img.shields.io/badge/ASP.NET_Core-API-6DB33F?logo=asp.net&logoColor=white" alt="ASP.NET Core" />
<img src="https://img.shields.io/badge/Dapper-Dapper-1F8ACB?logo=datawire&logoColor=white" alt="Dapper" />
<img src="https://img.shields.io/badge/SQL_Server-SQLServer-CC2927?logo=microsoft-sql-server&logoColor=white" alt="SQL Server" />
<img src="https://img.shields.io/badge/Tests-xUnit-FF4088?logo=xunit&logoColor=white" alt="xUnit" />


API REST en .NET 8 con arquitectura en capas: `API`, `Application`, `Domain` e `Infrastructure`. Implementa operaciones CRUD para Productos, Categorías, Proveedores y Pedidos, usando Dapper para acceso a SQL Server y pruebas (unitarias e integración).

## Resumen

- Arquitectura limpia y separada por responsabilidades:
  - `API`: Controladores y configuración del host (Swagger, DI).
  - `Application`: Servicios, DTOs e interfaces de repositorio (lógica de aplicación).
  - `Domain`: Entidades del dominio (`Product`, `Category`, `Supplier`, `Order`, `OrderDetail`).
  - `Infrastructure`: Implementaciones de repositorios con Dapper y `DapperContext` para `SqlConnection`.
  - `Tests`: Pruebas unitarias e integración.


## Diagrama de referencias entre proyectos

```mermaid
graph LR
    Domain[Domain]
    Application[Application] --> Domain
    Infrastructure[Infrastructure] --> Application
    Infrastructure --> Domain
    API[API] --> Application
    API --> Infrastructure
    Tests[Tests] --> Application
    Tests --> Infrastructure
    Tests --> Domain
```

## Base de datos: Northwind
Northwind es una base de datos de muestra que contiene datos ficticios para una tienda de productos alimenticios.
- Relación entre tablas:
  - `Products` tiene una relación de muchos a uno con `Categories` y `Suppliers`.
  - `OrderDetails` tiene una relación de muchos a uno con `Orders` y `Products`.
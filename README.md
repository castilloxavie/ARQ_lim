# ARQ_lim — Refactorización con Clean Architecture

Proyecto de refactorización aplicando los principios de **Clean Architecture** y análisis estático con **SonarCloud**.

[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=castilloxavie_arq-lim&metric=alert_status)](https://sonarcloud.io/dashboard?id=castilloxavie_arq-lim)
[![Security Rating](https://sonarcloud.io/api/project_badges/measure?project=castilloxavie_arq-lim&metric=security_rating)](https://sonarcloud.io/dashboard?id=castilloxavie_arq-lim)
[![Maintainability Rating](https://sonarcloud.io/api/project_badges/measure?project=castilloxavie_arq-lim&metric=sqale_rating)](https://sonarcloud.io/dashboard?id=castilloxavie_arq-lim)

---

## Descripción

Este proyecto demuestra la aplicación práctica de **Clean Architecture** en una API .NET 8, refactorizando un proyecto inicial con múltiples problemas de diseño, seguridad y mantenibilidad.

### Objetivo

Transformar un proyecto con acoplamiento fuerte y vulnerabilidades en uno que cumpla con:

- **Separación de responsabilidades** por capas.
- **Inversión de dependencias** mediante interfaces.
- **Seguridad** en consultas SQL y manejo de secretos.
- **Buenas prácticas** de código mantenible y testeable.

---

## Arquitectura

El proyecto sigue cuatro capas concéntricas:

```
        WebApi  (Presentacion)
       /      \
      v        v
Application   Infrastructure
      \        /
       v      v
        Domain
```

| Capa | Responsabilidad | Ejemplos |
|------|-----------------|----------|
| **Domain** | Entidades y reglas de negocio | Order, OrderService, IAppLogger, IOrderRepository |
| **Application** | Casos de uso | CreateOrderHandler |
| **Infrastructure** | Implementaciones tecnicas | SqlOrderRepository, ConsoleLogger |
| **WebApi** | Endpoints HTTP y DI | Program.cs |

### Regla de Dependencias

- Domain no conoce a nadie.
- Application solo conoce a Domain.
- Infrastructure conoce a Domain (para implementar interfaces).
- WebApi conoce a Application e Infrastructure (composition root).

---

## Métricas SonarCloud

| Métrica | Antes | Después |
|---------|-------|---------|
| Open Issues | 16 | 0 |
| Security Rating | E | A |
| Security Issues | 7 | 0 |
| Reliability Issues | 1 | 0 |
| Maintainability Issues | 8 | 0 |
| Quality Gate | — | PASSED |

Dashboard completo: https://sonarcloud.io/dashboard?id=castilloxavie_arq-lim

---

## Cambios principales

### Bloque A - Clean Architecture
- Inversion de dependencias con interfaces IAppLogger e IOrderRepository.
- Eliminacion de dependencias circulares en los .csproj.
- Inyeccion de dependencias en Program.cs.

### Bloque B - Seguridad
- SQL Injection eliminado: consultas parametrizadas en SqlOrderRepository.
- Credenciales fuera del codigo: uso de User Secrets.
- CORS restringido: solo origenes confiables.
- Endpoint /info eliminado: ya no expone secretos.
- RandomNumberGenerator en lugar de Random.

### Bloque C - Buenas practicas
- Migracion a Microsoft.Data.SqlClient.
- Eliminacion de Thread.Sleep y codigo muerto.
- Manejo de errores global con JSON estructurado.

---

## Estructura del proyecto

```
ARQ_lim/
|-- src/
|   |-- Domain/
|   |   |-- Entities/         Order.cs
|   |   |-- Interfaces/       IAppLogger.cs, IOrderRepository.cs
|   |   |-- Services/         OrderService.cs
|   |-- Application/
|   |   |-- UseCases/         CreateOrder.cs
|   |-- Infrastructure/
|   |   |-- Data/             SqlOrderRepository.cs
|   |   |-- Logging/          ConsoleLogger.cs
|   |-- WebApi/
|       |-- Program.cs
|-- BadCleanArch.sln
|-- informe_final.md
|-- README.md
```

---

## Tecnologías

- .NET 8
- ASP.NET Core Minimal APIs
- Microsoft.Data.SqlClient 7.1.1
- SonarCloud (analisis estatico)
- GitHub (control de versiones)

---

## Cómo ejecutar

### Requisitos
- .NET 8 SDK
- SQL Server (local o Docker)
- Git

### Pasos

```bash
# 1. Clonar
git clone https://github.com/castilloxavie/ARQ_lim.git
cd ARQ_lim

# 2. Configurar User Secrets
dotnet user-secrets init --project src/WebApi/WebApi.csproj
dotnet user-secrets set "ConnectionStrings:Sql" "Server=localhost;Database=master;User Id=sa;Password=TU_PASSWORD;TrustServerCertificate=True" --project src/WebApi/WebApi.csproj

# 3. Compilar
dotnet build

# 4. Ejecutar
dotnet run --project src/WebApi --urls "http://localhost:5000"
```

### Endpoints

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | /health | Health check |
| POST | /orders | Crear orden (body CSV: cliente,producto,qty,precio) |
| GET | /orders/last | Últimas 50 órdenes |

---

## Autor

**Xavier Alberto Castillo**

- GitHub: https://github.com/castilloxavie

---

## Licencia

Proyecto académico - Libre para uso educativo.

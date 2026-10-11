# Informe Final — Refactorización con Clean Architecture

**Proyecto**: ARQ_lim (BadCleanArch)
**Autor**: Xavier Alberto Castillo
**Asignatura**: Arquitectura de Software / Clean Architecture
**Fecha**: Octubre 2026
**Herramienta de análisis**: SonarCloud
**Repositorio**: [URL de tu GitHub]

---

## 1. Introducción a Clean Architecture

### 1.1 ¿Qué es Clean Architecture?

Clean Architecture es una filosofía de diseño de software propuesta por Robert C. Martin ("Uncle Bob") que enfatiza la **separación de responsabilidades** en capas concéntricas. El objetivo principal es que el software sea:

- **Independiente de frameworks**: la lógica de negocio no depende de librerías externas.
- **Testeable**: se puede probar sin depender de base de datos, UI o servicios externos.
- **Independiente de la UI**: se puede cambiar la interfaz sin tocar la lógica.
- **Independiente de la base de datos**: se puede cambiar de SQL Server a PostgreSQL sin afectar el dominio.
- **Mantenible y evolutivo**: los cambios en una capa no impactan las demás.

### 1.2 Capas de Clean Architecture

El proyecto sigue cuatro capas concéntricas:

| Capa | Rol | Ejemplos en el proyecto |
|------|-----|-------------------------|
| **Domain** | Entidades y reglas de negocio puras | `Order`, `OrderService`, `IAppLogger`, `IOrderRepository` |
| **Application** | Casos de uso que orquestan el dominio | `CreateOrderHandler` |
| **Infrastructure** | Implementaciones técnicas | `SqlOrderRepository`, `ConsoleLogger` |
| **WebApi** | Presentación (endpoints HTTP) | `Program.cs` |

### 1.3 Regla de Dependencias

La **Regla de Dependencias** establece que las flechas de dependencia apuntan **hacia adentro**:

- **Domain** no conoce a nadie.
- **Application** solo conoce a Domain.
- **Infrastructure** conoce a Domain (para implementar interfaces).
- **WebApi** conoce a Application e Infrastructure (composition root).

Esto se logra mediante **Inversión de Dependencias (DIP)**: las capas internas definen **interfaces**, y las capas externas proveen las **implementaciones**.

### 1.4 Beneficios aplicados al proyecto

| Beneficio | Antes | Después |
|-----------|-------|---------|
| **Calidad** | Código con SQL Injection, Random no criptográfico, credenciales expuestas | Código con consultas parametrizadas, `RandomNumberGenerator`, User Secrets |
| **Mantenibilidad** | Acoplamiento fuerte, estado global estático | Capas desacopladas, inyección de dependencias |
| **Evolución** | Cambiar SQL Server requería tocar Domain | Cambiar el repositorio solo afecta Infrastructure |

---

## 2. Métricas de SonarQube

### 2.1 Estado inicial (línea base)

Al ejecutar el primer análisis en SonarCloud, el proyecto presentaba:

| Métrica | Valor |
|---------|-------|
| **Lines of Code** | 2,800 |
| **Open Issues** | 16 |
| **Security Rating** | **E** (crítico) |
| **Security Issues** | 7 (57% Blocker, 43% Medium) |
| **Reliability Rating** | A |
| **Reliability Issues** | 1 |
| **Maintainability Rating** | A |
| **Maintainability Issues** | 8 |
| **Coverage** | 0.0% |
| **Duplications** | 0.0% |
| **Technical Debt** | ~6h 25min |
| **Split Responsibilities** | 2 |

**Issues críticos identificados**:

- 🔴 Password hardcodeado en `BadDb.cs`, `Program.cs` y `appsettings.json`.
- 🔴 SQL Injection en `CreateOrder.cs` (concatenación de strings).
- 🔴 CORS abierto a todos los orígenes (`AllowAnyOrigin`).
- 🔴 Endpoint `/info` exponiendo connection string y variables de entorno.
- 🟠 Uso de `Random` no criptográfico (Sonar S2245).
- 🟠 `Thread.Sleep(1500)` bloqueando el hilo.
- 🟠 Dependencias circulares entre proyectos.

### 2.2 Estado después de la refactorización

Después de aplicar los cambios:

| Métrica | Antes | Después | Mejora |
|---------|-------|---------|--------|
| **Open Issues** | 16 | **0** | **-100%** |
| **Security Rating** | E | **A** | ✅ |
| **Security Issues** | 7 | **0** | -100% |
| **Reliability Rating** | A | A | = |
| **Reliability Issues** | 1 | **0** | -100% |
| **Maintainability Rating** | A | A | = |
| **Maintainability Issues** | 8 | **0** | -100% |
| **Duplications** | 0.0% | 0.0% | = |
| **Quality Gate** | — | **PASSED** | 🏆 |

**Capturas adjuntas**: `capturas/01_overview_antes.png`, `capturas/02_overview_despues.png`, etc.

---

## 3. Cambios Realizados

### 3.1 Bloque A — Inversión de Dependencias (Clean Architecture)

#### Problema
- `Domain` dependía de `Infrastructure` (violaba la Regla de Dependencias).
- `Application` dependía de `Infrastructure` y `WebApi` (dependencias circulares).
- `Order.cs` llamaba directamente a `Infrastructure.Logging.Logger`.
- `OrderService` era una clase estática con estado global (`List<Order> LastOrders`).
- `CreateOrder.cs` tenía SQL Injection y usaba `Thread.Sleep`.

#### Cambios
1. Se crearon las interfaces `IAppLogger` e `IOrderRepository` en `Domain/Interfaces/`.
2. Se refactorizó `Order.cs` para eliminar la dependencia de Infrastructure.
3. Se convirtió `OrderService` en una clase con inyección de dependencias.
4. Se refactorizó `CreateOrder.cs` (ahora `CreateOrderHandler`) para usar `IOrderRepository`.
5. Se crearon `ConsoleLogger` (Infrastructure) e `SqlOrderRepository` (Infrastructure).
6. Se ajustaron las referencias de los `.csproj`:
   - `Domain` → sin referencias.
   - `Application` → solo `Domain`.
   - `Infrastructure` → solo `Domain`.
   - `WebApi` → `Application` e `Infrastructure`.

#### Resultado
- Ciclo de dependencias eliminado.
- Grafo de arquitectura en SonarCloud **alineado con Clean Architecture**.

### 3.2 Bloque B — Seguridad

#### Problemas
- Password `SuperSecret123!` hardcodeado en 3 archivos.
- SQL Injection en la creación de órdenes.
- CORS `AllowAnyOrigin` (vulnerabilidad de seguridad).
- Endpoint `/info` exponiendo secretos.
- Uso de `Random` en vez de `RandomNumberGenerator`.

#### Cambios
1. **User Secrets**: se movió la connection string fuera del proyecto.
   ```powershell
   dotnet user-secrets init --project src\WebApi\WebApi.csproj
   dotnet user-secrets set "ConnectionStrings:Sql" "..." --project src\WebApi\WebApi.csproj